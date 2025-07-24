using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drosy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedCreatredByandCreatedAtColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Students",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "Students",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Students");
        }
    }
}
