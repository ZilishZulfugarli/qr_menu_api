using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MenuAPI.Migrations
{
    /// <inheritdoc />
    public partial class branches_updated4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuCategories_Branches_BranchId",
                table: "MenuCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuCategories_Menus_MenuId",
                table: "MenuCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Menus_Branches_BranchId",
                table: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_MenuCategories_BranchId",
                table: "MenuCategories");

            migrationBuilder.DropIndex(
                name: "IX_MenuCategories_MenuId",
                table: "MenuCategories");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "MenuCategories");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCategories_BranchMenuId",
                table: "MenuCategories",
                column: "BranchMenuId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCategories_Branches_BranchMenuId",
                table: "MenuCategories",
                column: "BranchMenuId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCategories_Menus_BranchMenuId",
                table: "MenuCategories",
                column: "BranchMenuId",
                principalTable: "Menus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Menus_Branches_BranchId",
                table: "Menus",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuCategories_Branches_BranchMenuId",
                table: "MenuCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuCategories_Menus_BranchMenuId",
                table: "MenuCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Menus_Branches_BranchId",
                table: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_MenuCategories_BranchMenuId",
                table: "MenuCategories");

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "MenuCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MenuCategories_BranchId",
                table: "MenuCategories",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCategories_MenuId",
                table: "MenuCategories",
                column: "MenuId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCategories_Branches_BranchId",
                table: "MenuCategories",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCategories_Menus_MenuId",
                table: "MenuCategories",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Menus_Branches_BranchId",
                table: "Menus",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
