using System;
namespace MenuAPI.Entities
{
	public class CategoryFoods
	{
        public int Id { get; set; }
        public string FoodName { get; set; }
        public string? ImageName { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int MenuCategoryId { get; set; }
        public MenuCategory MenuCategory { get; set; }
    }
}

