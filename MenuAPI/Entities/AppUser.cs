using System;
using Microsoft.AspNetCore.Identity;

namespace MenuAPI.Entities
{
	public class AppUser : IdentityUser
	{
        public string BrandName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ImageName { get; set; }
        public string? ConfirmationToken { get; set; }

        public List<Branch>? Branches { get; set; }
    }
}