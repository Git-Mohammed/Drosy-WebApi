using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drosy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemSettingEntitySeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "Configuration",
                table: "SystemSettings",
                columns: new[] { "Id", "DefaultCurrency", "LogoPath", "WebName" },
                values: new object[] { 1, "USD", null, "Drosy" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Configuration",
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
