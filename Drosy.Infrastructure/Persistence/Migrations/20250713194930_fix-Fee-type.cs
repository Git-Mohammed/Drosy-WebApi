using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drosy.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class fixFeetype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Fee",
                schema: "EduManagement",
                table: "PlanStudents",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Fee",
                schema: "EduManagement",
                table: "PlanStudents",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
