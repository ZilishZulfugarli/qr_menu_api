using System;
using System.ComponentModel.DataAnnotations;

namespace MenuAPI.DTOs
{
	public class UpdateAccountDto
	{
		public string? BrandName { get; set; }
		public string? ImageName { get; set; }
		public IFormFile? ImageFile { get; set; }
		[MinLength(6)]
        [DataType(DataType.Password, ErrorMessage = "Password's length must be 6 or longer")]
        public string? Password { get; set; }

	}
}

