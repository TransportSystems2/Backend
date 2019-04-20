using Microsoft.EntityFrameworkCore.Migrations;

namespace TransportSystems.Backend.API.Migrations
{
    public partial class PropertyOfPublicityInTheGarage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "Companies");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Garages",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Garages");

            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "Companies",
                nullable: false,
                defaultValue: false);
        }
    }
}
