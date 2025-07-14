using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drosy.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addplanstudenttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "EduManagement");

            migrationBuilder.CreateTable(
                name: "Plans",
                schema: "EduManagement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DaysOfWeek = table.Column<int>(type: "int", nullable: false),
                    TotalFees = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanStudents",
                schema: "EduManagement",
                columns: table => new
                {
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    Fee = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanStudents", x => new { x.PlanId, x.StudentId });
                    table.ForeignKey(
                        name: "FK_PlanStudents_Plans_PlanId",
                        column: x => x.PlanId,
                        principalSchema: "EduManagement",
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlanStudents_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanStudents_StudentId",
                schema: "EduManagement",
                table: "PlanStudents",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanStudents",
                schema: "EduManagement");

            migrationBuilder.DropTable(
                name: "Plans",
                schema: "EduManagement");
        }
    }
}
