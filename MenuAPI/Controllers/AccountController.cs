using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Security;
using MenuAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MenuAPI.DTOs;
using MenuAPI.Data;
using MimeKit;
using MenuAPI.Services.Abstract;
using Microsoft.VisualStudio.Web.CodeGeneration;
using QRCoder;
using static QRCoder.QRCodeGenerator;
using SkiaSharp;
using Microsoft.EntityFrameworkCore;

namespace MenuAPI.Controllers
{
    [Authorize(Roles = "User, Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        public AccountController(UserManager<AppUser> userManager, AppDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto dto, [FromServices] IUploadFileService uploadFile)
        {
            if (ModelState.IsValid)
            {
                if (_userManager.Users.Any(x => x.Email == dto.Email))
                    return BadRequest("User already exists");

                // Handle the file upload
                string? uploadedImagePath = null;
                if (dto.ImageFile != null)
                {
                    uploadedImagePath = await uploadFile.UploadFileAsync(dto.ImageFile, "wwwroot/images");
                }

                var user = new AppUser
                {
                    BrandName = dto.BrandName,
                    Email = dto.Email,
                    UserName = dto.Email,
                    ConfirmationToken = "",
                    CreatedDate = DateTime.UtcNow,
                    ImageName = uploadedImagePath
                };

                var result = await _userManager.CreateAsync(user, dto.Password);

                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                await _userManager.AddToRoleAsync(user, "User");

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = HttpUtility.UrlEncode(token);
                user.ConfirmationToken = token;
                await _userManager.UpdateAsync(user);

                var link = $"https://www.itag.az/activeaccount/{user.Id}/{encodedToken}";

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("ITAG Activation", "2002zilis@gmail.com"));
                message.To.Add(new MailboxAddress(user.BrandName, user.Email));
                message.Subject = "Email confirmation!";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                <p>Hello {user.BrandName},</p>
                <p>Thanks for registering with Menu API. Please click the button below to activate your account:</p>
                <p>
                    <a href='{link}' 
                       style='display: inline-block; padding: 10px 20px; font-size: 16px; color: #fff; background-color: #007bff; text-decoration: none; border-radius: 5px;'>
                        Activate Your Account
                    </a>
                </p>
                <p>If you didn't request to register with ITAG, please ignore this email.</p>
            "
                };

                message.Body = bodyBuilder.ToMessageBody();
                using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                {
                    smtp.Connect("smtp.gmail.com", 465, SecureSocketOptions.Auto);
                    smtp.Authenticate("2002zilis@gmail.com", "okuf lkkk tznr forb");
                    smtp.Send(message);
                    smtp.Disconnect(true);
                }

                return Ok(new { user, token });
            }

            return BadRequest();
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto, [FromServices] IJwtTokenService tokenService)
        {
            if (!ModelState.IsValid)
            {
                var message = ModelState
                   .SelectMany(modelState => modelState.Value!.Errors)
                   .Select(err => err.ErrorMessage)
                   .ToList();

                return BadRequest(message);
            }

            var user = await _userManager.FindByEmailAsync(dto.Email!);
            if (user is null) return NotFound("User not found");
            //if (user.EmailConfirmed == false)
            //{
            //    return BadRequest("User isn't active!");
            //}

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, dto.Password!);
            if (!isPasswordCorrect) return NotFound("Password is not correct");

            var roles = (await _userManager.GetRolesAsync(user)).ToList();

            var userToken = tokenService.GenerateToken(user.BrandName!, user.UserName!, roles, user.Id);

            if (userToken is null) return NotFound();

            return Ok(new { token = userToken, userId = user.Id, role = roles, userInfo = user});
        }

        [HttpPost("Update")]
        public async Task<IActionResult> UpdateAccount(UpdateAccountDto dto, [FromServices] IUploadFileService uploadFile)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("User not authenticated.");

            var account = _dbContext.Users.FirstOrDefault(u => u.Id == userId);

            if (!string.IsNullOrEmpty(dto.BrandName))
                account.BrandName = dto.BrandName;

            if (!string.IsNullOrEmpty(dto.Password))
            {
                var passwordHasher = new PasswordHasher<AppUser>();

                account.PasswordHash = passwordHasher.HashPassword(account, dto.Password);
            }

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(account.ImageName))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", account.ImageName);
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }

                var newImageName = await uploadFile.UploadFileAsync(dto.ImageFile, "uploads");
                account.ImageName = newImageName;
            }

            _dbContext?.SaveChangesAsync();

            return Ok("Account updated successfully!");
        }

        [HttpDelete("DeleteAccount")]
        public IActionResult DeleteAccount()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("User not authenticated.");

            var account = _dbContext.Users.FirstOrDefault(u => u.Id == userId);

            _dbContext?.Remove(account);
            _dbContext?.SaveChanges();

            return Ok("Account deleted!");
        }

        [HttpPost("CreateQR")]
        public IActionResult CreateQR([FromQuery] int id, [FromQuery] string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
            {
                return BadRequest("You must add an email address!");
            }

            var menu = _dbContext.Menus.Include(x => x.Branch).FirstOrDefault(x => x.Id == id);

            if (menu == null)
            {
                return NotFound("Menu not found.");
            }

            var menuName = menu.Branch.BranchName;

            string qrContent = $"{menuName}-in QR kodu";

            var qrCode = GenerateQrCode(qrContent);

            SendEmailWithQrCode(emailAddress, qrCode);

            return Ok("QR code created and sent to email.");
        }

        private byte[] GenerateQrCode(string content)
        {
            var generator = new QRCodeGenerator();
            var qr = generator.CreateQrCode(content, ECCLevel.Q);

            int moduleCount = qr.ModuleMatrix.Count;
            int size = moduleCount * 20; 

            var info = new SKImageInfo(size, size);
            using (var surface = SKSurface.Create(info))
            {
                var canvas = surface.Canvas;
                canvas.Clear(SKColors.White);

                int moduleSize = size / moduleCount;

                using (var paint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = SKColors.Black
                })
                {
                    for (int y = 0; y < moduleCount; y++)
                    {
                        for (int x = 0; x < moduleCount; x++)
                        {
                            if (qr.ModuleMatrix[y][x])
                            {
                                canvas.DrawRect(new SKRect(x * moduleSize, y * moduleSize, (x + 1) * moduleSize, (y + 1) * moduleSize), paint);
                            }
                        }
                    }
                }

                using (var image = surface.Snapshot())
                {
                    using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                    {
                        return data.ToArray();
                    }
                }
            }
        }

        private void SendEmailWithQrCode(string emailAddress, byte[] qrCode)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Mytag", "nurayib.esger@gmail.com"));
            message.To.Add(new MailboxAddress("Zilish", emailAddress));
            message.Subject = "QR Code";

            var bodyBuilder = new BodyBuilder { TextBody = "Here is your Menu API QR code." };

            var attachment = new MimePart("image", "png")
            {
                Content = new MimeContent(new MemoryStream(qrCode)),
                ContentDisposition = new MimeKit.ContentDisposition(MimeKit.ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = "menu_qr_code.png"
            };

            bodyBuilder.Attachments.Add(attachment);

            message.Body = bodyBuilder.ToMessageBody();

            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                smtp.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect); 
                smtp.Authenticate("2002zilis@gmail.com", "okuf lkkk tznr forb"); 
                smtp.Send(message);
                smtp.Disconnect(true);
            }
        }

    }
}
