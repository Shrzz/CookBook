using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cookbook.data.migrations
{
    public partial class UpdatedImagesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Recipes_RecipeId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_RecipeId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "RecipeId",
                table: "Images");

            migrationBuilder.AddColumn<string>(
                name: "Images",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Images",
                table: "Recipes");

            migrationBuilder.AddColumn<int>(
                name: "RecipeId",
                table: "Images",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_RecipeId",
                table: "Images",
                column: "RecipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Recipes_RecipeId",
                table: "Images",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id");
        }
    }
}
