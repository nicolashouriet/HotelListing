using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelListing.Data.Migrations
{
    public partial class defaultroles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "0cb57121-2bb9-454e-a674-2ba4fc28b069", "3f640400-be67-4942-b02f-61e9f0f8408c", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "ec999d94-ffd0-4db6-9796-4f1cc0b7e286", "f72a0a80-9e2a-40b9-b5ae-70b58560ed3b", "User", "USER" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0cb57121-2bb9-454e-a674-2ba4fc28b069");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ec999d94-ffd0-4db6-9796-4f1cc0b7e286");
        }
    }
}
