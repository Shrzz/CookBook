using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cookbook.data.migrations
{
    public partial class ChangedImageStorageMethod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Images",
                table: "Recipes",
                newName: "ImagesDirectory");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagesDirectory",
                table: "Recipes",
                newName: "Images");
        }
    }
}
