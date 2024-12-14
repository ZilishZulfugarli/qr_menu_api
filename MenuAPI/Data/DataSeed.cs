using System;
using MenuAPI.Entities;
using Microsoft.AspNetCore.Identity;

namespace MenuAPI.Data
{
	public class DataSeed
	{
        public static async Task InitializeAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>(); // Resolve the AppDbContext

            var roles = new string[] { "Admin", "User" };

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in roles)
            {
                var existingRole = await roleManager.FindByNameAsync(role);
                if (existingRole == null)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            string adminUserName = configuration["DefaultAdmin:UserName"]!;
            string adminPassword = configuration["DefaultAdmin:Password"]!;

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var adminUser = await userManager.FindByNameAsync(adminUserName);

            if (adminUser != null) return;


            adminUser = new AppUser
            {
                BrandName = "Zilish",
                Email = adminUserName,
                UserName = adminUserName,
                ConfirmationToken = "",
                CreatedDate = DateTime.UtcNow,
                EmailConfirmed = true,
            };

            var createResult = await userManager.CreateAsync(adminUser, adminPassword);

            if (createResult.Succeeded)
            {
                var token = await userManager.GenerateEmailConfirmationTokenAsync(adminUser);

                adminUser.ConfirmationToken = token;
                await userManager.UpdateAsync(adminUser);

                await userManager.ConfirmEmailAsync(adminUser, token);

                await userManager.AddToRoleAsync(adminUser, roles[0]);
            }
            else
            {
                foreach (var error in createResult.Errors)
                {
                    Console.WriteLine($"Error creating admin: {error.Description}");
                }
            }
        }
    }
}

