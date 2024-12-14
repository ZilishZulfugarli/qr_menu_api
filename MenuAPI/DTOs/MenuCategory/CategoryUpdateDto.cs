using System;
namespace MenuAPI.DTOs.MenuCategory
{
    public class CategoryUpdateDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int BranchId { get; set; }
    }
}

