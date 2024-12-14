using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MenuAPI.Data;
using MenuAPI.DTOs.CategoryFoods;
using MenuAPI.DTOs.Menu;
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
    public class MenuController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        public MenuController(UserManager<AppUser> userManager, AppDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [HttpGet("GetBranchMenus")]
        public IActionResult GetBranchMenus([FromQuery] int id)
        {
            var branch = _dbContext.Branches.FirstOrDefault(x => x.Id == id);

            if (branch == null)
            {
                return NotFound("Doesn't exist any branch!");
            }

            var menus = _dbContext.Menus
                .Include(x => x.MenuCategories)
                .ThenInclude(x => x.CategoryFoods)
                .Where(x => x.BranchId == branch.Id)
                .Select(menu => new MenuDto
                {
                    BranchId = menu.BranchId,
                    CategoryDtos = menu.MenuCategories.Select(category => new CategoryDto
                    {
                        Id = category.Id,
                        Name = category.Name,
                        FoodDtos = category.CategoryFoods.Select(food => new FoodDto
                        {
                            Id = food.Id,
                            FoodName = food.FoodName,
                            Description = food.Description,
                            Price = food.Price,
                            DiscountPrice = food.DiscountPrice ?? 0,
                            ImageName = food.ImageName,
                        }).ToList()
                    }).ToList()
                }).ToList();

            if (!menus.Any())
            {
                return NotFound("Doesn't exist any menu!");
            }

            return Ok(menus);
        }


        [HttpPost("AddMenu")]
        public IActionResult AddMenu([FromBody] MenuAddDto dto)
        {
            var branch = _dbContext.Branches.FirstOrDefault(x => x.Id == dto.BranchId);

            if (branch == null)
            {
                return NotFound("Doesn't exist any branch!");
            }
            var menu = new Menu()
            {
                Branch = branch,
                BranchId = branch.Id
            };

            _dbContext.Add(menu);
            _dbContext.SaveChanges();

            return Ok("Menu added successfully!");
        }

        [HttpPut("UpdateMenu")]
        public IActionResult UpdateMenu([FromBody] MenuUpdateDto dto)
        {
            var menu = _dbContext.Menus.Include(x => x.Branch).FirstOrDefault(x => x.Id == dto.MenuId);

            if (menu == null)
                return NotFound("Menu not found.");

            if (dto.BranchId > 0 && dto.BranchId != menu.BranchId)
            {
                var branch = _dbContext.Branches.FirstOrDefault(x => x.Id == dto.BranchId);
                if (branch == null)
                    return NotFound("Branch not found.");
                menu.BranchId = branch.Id;
                menu.Branch = branch;
            }

            _dbContext.SaveChanges();

            return Ok("Menu updated successfully.");
        }

        [HttpDelete("DeleteMenu")]
        public IActionResult DeleteMenu([FromQuery] int menuId)
        {
            var menu = _dbContext.Menus.Include(x => x.MenuCategories)
                                       .ThenInclude(x => x.CategoryFoods)
                                       .FirstOrDefault(x => x.Id == menuId);

            if (menu == null)
                return NotFound("Menu not found.");

            if (menu.MenuCategories != null)
            {
                foreach (var category in menu.MenuCategories)
                {
                    if (category.CategoryFoods != null)
                        _dbContext.CategoryFoods.RemoveRange(category.CategoryFoods);

                    _dbContext.MenuCategories.Remove(category);
                }
            }

            _dbContext.Menus.Remove(menu);
            _dbContext.SaveChanges();

            return Ok("Menu deleted successfully.");
        }
    }
}
