using System;
namespace MenuAPI.Entities
{
	public class MenuCategory
	{
        public int Id { get; set; }
        public string Name { get; set; }
        public int BranchMenuId { get; set; }
        public int MenuId { get; set; }
        public Menu Menu { get; set; }
        public int BranchId { get; set; }

        public Branch Branch { get; set; }
        public List<CategoryFoods> CategoryFoods { get; set; }
    }
}

