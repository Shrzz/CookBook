using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cookbook.data.migrations
{
    public partial class RecipeTablePropsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ingridients",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Steps",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ingridients",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "Steps",
                table: "Recipes");
        }
    }
}
