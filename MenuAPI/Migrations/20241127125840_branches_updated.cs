using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MenuAPI.Migrations
{
    /// <inheritdoc />
    public partial class branches_updated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "MenuCategories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuCategories_BranchId",
                table: "MenuCategories",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCategories_Branches_BranchId",
                table: "MenuCategories",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuCategories_Branches_BranchId",
                table: "MenuCategories");

            migrationBuilder.DropIndex(
                name: "IX_MenuCategories_BranchId",
                table: "MenuCategories");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "MenuCategories");
        }
    }
}
