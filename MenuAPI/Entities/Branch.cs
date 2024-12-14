using System;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace MenuAPI.Entities
{
	public class Branch
	{
        public int Id { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public TimeSpan? OpenDate { get; set; }
        public TimeSpan? CloseDate { get; set; }
        public string? Bio { get; set; }
        public decimal ServiceFee { get; set; }
        //public int BranchId { get; set; }
        //public int MenuCategoriesId { get; set; }

        public List<MenuCategory>? MenuCategories { get; set; }
        public AppUser User { get; set; }
    }
}

