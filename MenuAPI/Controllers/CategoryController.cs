using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MenuAPI.Data;
using MenuAPI.DTOs.MenuCategory;
using MenuAPI.Entities;
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
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        public CategoryController(UserManager<AppUser> userManager, AppDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }


        [HttpPost("AddCategory")]
        public IActionResult AddCategory([FromBody] CategoryAddDto dto)
        {
            var menu = _dbContext.Menus.FirstOrDefault(x => x.Id == dto.BranchMenuId);

            if (menu == null)
                return NotFound("Menu not found.");

            var category = new MenuCategory
            {
                Name = dto.Name,
                BranchMenuId = menu.Id,
                Menu = menu
            };

            _dbContext.MenuCategories.Add(category);
            _dbContext.SaveChanges();

            return Ok("Category added successfully.");
        }

        [HttpPut("UpdateCategory")]
        public IActionResult UpdateCategory([FromBody] CategoryUpdateDto dto)
        {
            var category = _dbContext.MenuCategories.FirstOrDefault(x => x.Id == dto.Id);

            if (category == null)
                return NotFound("Category not found.");

            if (!string.IsNullOrEmpty(dto.Name))
                category.Name = dto.Name;

            //if (dto.BranchId != null)
            //{
            //    var branch = _dbContext.Branches.FirstOrDefault(x => x.Id == dto.BranchId);

            //    if (branch == null)
            //    {
            //        return NotFound("Branch doesn't exist!");
            //    }

            //    category.BranchMenuId = dto.BranchId
            //}
                

            _dbContext.SaveChanges();

            return Ok("Category updated successfully.");
        }

        [HttpDelete("DeleteCategory")]
        public IActionResult DeleteCategory([FromQuery] int categoryId)
        {
            var category = _dbContext.MenuCategories
                .Include(x => x.CategoryFoods)
                .FirstOrDefault(x => x.Id == categoryId);

            if (category == null)
                return NotFound("Category not found.");

            // If `CategoryFoods` are related, delete them
            if (category.CategoryFoods != null && category.CategoryFoods.Any())
                _dbContext.CategoryFoods.RemoveRange(category.CategoryFoods);

            _dbContext.MenuCategories.Remove(category);
            _dbContext.SaveChanges();

            return Ok("Category deleted successfully.");
        }
    }
}
