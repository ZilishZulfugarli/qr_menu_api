using System;
using MenuAPI.DTOs.CategoryFoods;

namespace MenuAPI.DTOs.MenuCategory
{
	public class CategoryDto
	{
        public int Id { get; set; }
        public int BranchMenuId { get; set; }
        public string Name { get; set; }
        public List<FoodDto> FoodDtos { get; set; }
    }
}

