
using System;
using MenuAPI.DTOs.MenuCategory;

namespace MenuAPI.DTOs.Menu
{
	public class MenuDto
	{
		public int BranchId { get; set; }
		public List<CategoryDto> CategoryDtos { get; set; }
	}
}

