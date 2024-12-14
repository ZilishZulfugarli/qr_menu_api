using System;
namespace MenuAPI.Entities
{
	public class Menu
	{
        public int Id { get; set; }

        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        public List<MenuCategory>? MenuCategories { get; set; }
    }
}

