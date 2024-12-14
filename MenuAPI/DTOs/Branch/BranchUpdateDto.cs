using System;
namespace MenuAPI.DTOs.Branch
{
	public class BranchUpdateDto
	{
        public int Id { get; set; }
        public string? BranchName { get; set; }
        public string? BranchAddress { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public string? Bio { get; set; }
        public decimal ServiceFee { get; set; }
    }
}

