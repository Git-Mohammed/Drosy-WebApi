using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Drosy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addRegionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "Cities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 1,
                column: "RegionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CountryId", "Name", "RegionId" },
                values: new object[] { 1, "مأرب", null });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "CountryId", "Name", "RegionId" },
                values: new object[] { 3, 1, "عدن", null });

            migrationBuilder.InsertData(
                table: "Regions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Ma’rib" },
                    { 2, "Sana’a" },
                    { 3, "Aden" },
                    { 4, "Ta’izz" },
                    { 5, "Hadramawt" },
                    { 6, "Al Hudaydah" },
                    { 7, "Ibb" },
                    { 8, "Shabwah" },
                    { 9, "Al Mahrah" },
                    { 10, "Al Jawf" },
                    { 11, "Amran" },
                    { 12, "Dhamar" },
                    { 13, "Raymah" },
                    { 14, "Sa’dah" },
                    { 15, "Lahij" },
                    { 16, "Al Bayda" },
                    { 17, "Abyan" },
                    { 18, "Al Dhale’e" },
                    { 19, "Socotra" },
                    { 20, "Hajjah" },
                    { 21, "Mahwit" },
                    { 22, "Sana’a City" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cities_RegionId",
                table: "Cities",
                column: "RegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Regions_RegionId",
                table: "Cities",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Regions_RegionId",
                table: "Cities");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.DropIndex(
                name: "IX_Cities_RegionId",
                table: "Cities");

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "Cities");

            migrationBuilder.UpdateData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CountryId", "Name" },
                values: new object[] { 2, "الرياض" });
        }
    }
}
