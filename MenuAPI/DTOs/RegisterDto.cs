using System;
using System.ComponentModel.DataAnnotations;

namespace MenuAPI.DTOs
{
	public class RegisterDto
	{
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email is wrong.")]
        public string? Email { get; set; }
        [Required]
        [DataType(DataType.Password, ErrorMessage = "Password is wrong")]
        public string? Password { get; set; }
        [Required]
        [MinLength(3)]
        public string BrandName { get; set; }

        public string? ImageName { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}

