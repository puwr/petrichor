using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImageDimensionsAndRenameUserIdToUploaderId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Images",
                newName: "UploaderId");

            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "Images",
                newName: "UploadedDateTime");

            migrationBuilder.RenameColumn(
                name: "CreatedDateTime",
                table: "Images",
                newName: "OriginalImagePath");

            migrationBuilder.AddColumn<int>(
                name: "OriginalImageHeight",
                table: "Images",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OriginalImageWidth",
                table: "Images",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ThumbnailHeight",
                table: "Images",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ThumbnailWidth",
                table: "Images",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_Name",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "OriginalImageHeight",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "OriginalImageWidth",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "ThumbnailHeight",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "ThumbnailWidth",
                table: "Images");

            migrationBuilder.RenameColumn(
                name: "UploaderId",
                table: "Images",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "UploadedDateTime",
                table: "Images",
                newName: "ImagePath");

            migrationBuilder.RenameColumn(
                name: "OriginalImagePath",
                table: "Images",
                newName: "CreatedDateTime");
        }
    }
}
