using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductPictureAndThumbnail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PicturePath",
                table: "Products",
                newName: "PictureName");

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailName",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PictureName",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //This is messed up, dont try to downdate database
            migrationBuilder.DropColumn(
                name: "PictureName",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ThumbnailName",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "PicturePath",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
