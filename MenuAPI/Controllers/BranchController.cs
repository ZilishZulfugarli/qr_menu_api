using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MenuAPI.Data;
using MenuAPI.DTOs.Branch;
using MenuAPI.Entities;
using MenuAPI.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MenuAPI.Controllers
{
    [Authorize(Roles = "User, Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        public BranchController(UserManager<AppUser> userManager, AppDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [AllowAnonymous]
        [HttpGet("GetBranchById")]
        public IActionResult GetBranchById([FromQuery] int id)
        {
            if (id == null)
            {
                return BadRequest("Pls add id!");
            }

            var branch = _dbContext.Branches.Include(x => x.MenuCategories).ThenInclude(x => x.CategoryFoods).AsNoTracking().ToList();

            if (branch == null)
            {
                return NotFound("Doesn't Exist Ant Branch!");
            }

            return Ok(branch);
        }

        [HttpGet("GetAllBranches")]
        public IActionResult GetAllBranches()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return NotFound("Account doesn't exist!");
            }

            var user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);

            var branches = _dbContext.Branches.Include(x => x.User).AsNoTracking().Where(x => x.User.Id == user.Id).ToList();


            if (!branches.Any())
            {
                return NotFound("Doesn't exist any branch!");
            }

            var response = branches.Select(branch => new
            {
                branch.Id,
                branch.BranchName,
                branch.BranchAddress,
                branch.OpenDate,
                branch.CloseDate,
                branch.ServiceFee,
                branch.Bio
            });

            return Ok(response);
        }

        [HttpPost("AddBranch")]
        public IActionResult AddBranch([FromBody] BranchAddDto dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);

            if (user == null)
            {
                return NotFound("Account doesn't exist!");
            }

            var branch = new Branch()
            {
                BranchName = dto.BranchName,
                OpenDate = dto.OpenDate.TimeOfDay,
                CloseDate = dto.CloseDate.TimeOfDay,
                BranchAddress = dto.BranchAddress,
                Bio = dto.Bio,
                ServiceFee = dto.ServiceFee,
                User = user
            };

            _dbContext.Add(branch);
            _dbContext.SaveChanges();

            var response = new
            {
                branch.Id,
                branch.BranchName,
                branch.BranchAddress,
                branch.OpenDate,
                branch.CloseDate,
                branch.ServiceFee,
                branch.Bio
            };

            return Ok(response);
        }

        [HttpPut("ChangeBranch")]
        public async Task<IActionResult> ChangeBranch([FromForm] BranchUpdateDto dto)
        {
            var branch = _dbContext.Branches.FirstOrDefault(b => b.Id == dto.Id);

            if (branch == null)
                return NotFound("Branch not found.");

            if (!string.IsNullOrEmpty(dto.BranchName))
                branch.BranchName = dto.BranchName;

            if (!string.IsNullOrEmpty(dto.BranchAddress))
                branch.BranchAddress = dto.BranchAddress;

            if (dto.OpenDate != default && dto.OpenDate != null)
                branch.OpenDate = dto.OpenDate?.TimeOfDay;

            if (dto.CloseDate != default && dto.OpenDate != null)
                branch.CloseDate = dto.CloseDate?.TimeOfDay;

            if (!string.IsNullOrEmpty(dto.Bio))
                branch.Bio = dto.Bio;

            if (dto.ServiceFee > 0)
                branch.ServiceFee = dto.ServiceFee;

            _dbContext.SaveChanges();

            return Ok("Branch updated successfully.");
        }

        [HttpDelete("DeleteBranch")]
        public IActionResult DeleteBranch([FromQuery] int id)
        {
            var branch = _dbContext.Branches.FirstOrDefault(x => x.Id == id);

            if (branch == null)
            {
                return NotFound("Branch doesn't exist!");
            }

            _dbContext.Remove(branch);
            _dbContext.SaveChanges();

            return Ok($"{branch.BranchName} deleted successfully!");
        }

    }
}
