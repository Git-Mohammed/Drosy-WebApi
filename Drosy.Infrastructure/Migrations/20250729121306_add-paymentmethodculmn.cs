using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drosy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addpaymentmethodculmn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "Finance",
                table: "Payments",
                type: "datetime2",
                nullable: false,
                comment: "Timestamp indicating when the payment was created",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                schema: "Finance",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: false,
                comment: "The monetary amount paid by the student",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "Method",
                schema: "Finance",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Indicates how the payment was made (1 = Cash, 2 = BankTransfer, 3 = CreditCard, 4 = MobilePayment, 5 = Scholarship, 6 = Other)");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                schema: "Finance",
                table: "Payments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "Optional notes or remarks attached to the payment");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Method",
                schema: "Finance",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Notes",
                schema: "Finance",
                table: "Payments");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "Finance",
                table: "Payments",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Timestamp indicating when the payment was created");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                schema: "Finance",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "The monetary amount paid by the student");
        }
    }
}
