using System;
using MenuAPI.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace MenuAPI.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuCategory> MenuCategories { get; set; }
        public DbSet<CategoryFoods> CategoryFoods { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<MenuCategory>()
        //        .HasOne(mc => mc.Branch)
        //        .WithMany(b => b.MenuCategories)
        //        .HasForeignKey(mc => mc.BranchMenuId)
        //        .OnDelete(DeleteBehavior.Restrict); 

        //    modelBuilder.Entity<Menu>()
        //        .HasOne(m => m.Branch)
        //        .WithMany(n => n.MenuCategories)
        //        .HasForeignKey(m => m.BranchId)
        //        .OnDelete(DeleteBehavior.Restrict);

        //    modelBuilder.Entity<MenuCategory>()
        //        .HasOne(mc => mc.Menu)
        //        .WithMany(m => m.MenuCategories)
        //        .HasForeignKey(mc => mc.BranchMenuId)
        //        .OnDelete(DeleteBehavior.Restrict);

        //    modelBuilder.Entity<CategoryFoods>()
        //        .HasOne(cf => cf.MenuCategory)
        //        .WithMany(mc => mc.CategoryFoods)
        //        .HasForeignKey(cf => cf.MenuCategoryId)
        //        .OnDelete(DeleteBehavior.Cascade);
        //}

    }
}

