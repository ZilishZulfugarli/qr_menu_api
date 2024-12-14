using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MenuAPI.Migrations
{
    /// <inheritdoc />
    public partial class branches_updated2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuCategories_Branches_BranchId",
                table: "MenuCategories");

            migrationBuilder.AlterColumn<int>(
                name: "BranchId",
                table: "MenuCategories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCategories_Branches_BranchId",
                table: "MenuCategories",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuCategories_Branches_BranchId",
                table: "MenuCategories");

            migrationBuilder.AlterColumn<int>(
                name: "BranchId",
                table: "MenuCategories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCategories_Branches_BranchId",
                table: "MenuCategories",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");
        }
    }
}
