﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MenuAPI.Migrations
{
    /// <inheritdoc />
    public partial class branches_updated3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Branches",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Branches");
        }
    }
}