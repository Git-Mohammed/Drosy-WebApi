using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drosy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_Split_PlanDayToTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaysOfWeek",
                schema: "EduManagement",
                table: "Plans");

            migrationBuilder.DropColumn(
                name: "EndSession",
                schema: "EduManagement",
                table: "Plans");

            migrationBuilder.DropColumn(
                name: "StartSession",
                schema: "EduManagement",
                table: "Plans");

            migrationBuilder.CreateTable(
                name: "PlanDays",
                schema: "EduManagement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    Day = table.Column<byte>(type: "tinyint", nullable: false),
                    StartSession = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndSession = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanDays_Plans_PlanId",
                        column: x => x.PlanId,
                        principalSchema: "EduManagement",
                        principalTable: "Plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanDays_PlanId",
                schema: "EduManagement",
                table: "PlanDays",
                column: "PlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanDays",
                schema: "EduManagement");

            migrationBuilder.AddColumn<int>(
                name: "DaysOfWeek",
                schema: "EduManagement",
                table: "Plans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndSession",
                schema: "EduManagement",
                table: "Plans",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartSession",
                schema: "EduManagement",
                table: "Plans",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }
    }
}
