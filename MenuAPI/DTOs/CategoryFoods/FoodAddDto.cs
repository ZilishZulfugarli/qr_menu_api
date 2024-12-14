using System;
using System.ComponentModel.DataAnnotations;

namespace MenuAPI.DTOs.CategoryFoods
{
	public class FoodAddDto
	{
        public string FoodName { get; set; }
        public string? ImageName { get; set; }
        public IFormFile ImageFile { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; }
        public int MenuCategoryId { get; set; }
    }
}

