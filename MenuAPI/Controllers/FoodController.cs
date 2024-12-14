using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MenuAPI.Data;
using MenuAPI.DTOs.CategoryFoods;
using MenuAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MenuAPI.Controllers
{
    [Authorize(Roles = "User, Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        public FoodController(UserManager<AppUser> userManager, AppDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [HttpPost("AddFood")]
        public IActionResult AddFood([FromForm] FoodAddDto dto)
        {
            var category = _dbContext.MenuCategories.FirstOrDefault(x => x.Id == dto.MenuCategoryId);

            if (category == null)
                return NotFound("Category not found.");

            string imagePath = null;

            if (dto.ImageFile != null)
            {
                var fileName = $"{Guid.NewGuid()}_{dto.ImageFile.FileName}";

                var filePath = Path.Combine("wwwroot/images", fileName);

                if (!Directory.Exists("wwwroot/images"))
                {
                    Directory.CreateDirectory("wwwroot/images");
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    dto.ImageFile.CopyTo(stream); 
                }

                imagePath = fileName;
            }

            var food = new CategoryFoods
            {
                FoodName = dto.FoodName,
                ImageName = imagePath, 
                Description = dto.Description,
                Price = dto.Price,
                DiscountPrice = dto.DiscountPrice > 0 ? dto.DiscountPrice : null,
                MenuCategoryId = category.Id,
                MenuCategory = category
            };

            _dbContext.CategoryFoods.Add(food);
            _dbContext.SaveChanges();

            return Ok("Food added successfully.");
        }

        [HttpPut("UpdateFood")]
        public IActionResult UpdateFood([FromForm] FoodUpdateDto dto)
        {
            var food = _dbContext.CategoryFoods.FirstOrDefault(x => x.Id == dto.Id);

            if (food == null)
                return NotFound("Food not found.");

            if (!string.IsNullOrEmpty(dto.FoodName))
                food.FoodName = dto.FoodName;

            if (dto.ImageName != null)
            {
                var oldImagePath = Path.Combine("wwwroot/images/foods", food.ImageName);
                if (System.IO.File.Exists(oldImagePath))
                    System.IO.File.Delete(oldImagePath);

                var fileName = $"{Guid.NewGuid()}_{dto.ImageName.FileName}";
                var filePath = Path.Combine("wwwroot/images/foods", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    dto.ImageName.CopyTo(stream);
                }
                food.ImageName = fileName;
            }

            if (!string.IsNullOrEmpty(dto.Description))
                food.Description = dto.Description;

            if (dto.Price > 0)
                food.Price = dto.Price;

            if (dto.DiscountPrice > 0)
                food.DiscountPrice = dto.DiscountPrice;

            if (dto.MenuCategoryId != 0)
            {
                var category = _dbContext.MenuCategories.FirstOrDefault(x => x.Id == dto.MenuCategoryId);
                if (category != null)
                    food.MenuCategoryId = dto.MenuCategoryId;
            }

            _dbContext.SaveChanges();

            return Ok("Food updated successfully.");
        }

        [HttpDelete("DeleteFood")]
        public IActionResult DeleteFood([FromQuery] int id)
        {
            var food = _dbContext.CategoryFoods.FirstOrDefault(x => x.Id == id);

            if (food == null)
                return NotFound("Food not found.");

            // Delete the image file if it exists
            if (!string.IsNullOrEmpty(food.ImageName))
            {
                var filePath = Path.Combine("wwwroot/images/foods", food.ImageName);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            _dbContext.CategoryFoods.Remove(food);
            _dbContext.SaveChanges();

            return Ok("Food deleted successfully.");
        }

    }
}
