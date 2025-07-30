using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Petrichor.Modules.Gallery.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "gallery");

            migrationBuilder.CreateTable(
                name: "images",
                schema: "gallery",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalImagePath = table.Column<string>(type: "text", nullable: false),
                    OriginalImageWidth = table.Column<int>(type: "integer", nullable: false),
                    OriginalImageHeight = table.Column<int>(type: "integer", nullable: false),
                    ThumbnailPath = table.Column<string>(type: "text", nullable: false),
                    ThumbnailWidth = table.Column<int>(type: "integer", nullable: false),
                    ThumbnailHeight = table.Column<int>(type: "integer", nullable: false),
                    uploader_id = table.Column<Guid>(type: "uuid", nullable: false),
                    uploaded_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_images", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                schema: "gallery",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tags", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "image_tags",
                schema: "gallery",
                columns: table => new
                {
                    images_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tags_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_image_tags", x => new { x.images_id, x.tags_id });
                    table.ForeignKey(
                        name: "fk_image_tags_images_images_id",
                        column: x => x.images_id,
                        principalSchema: "gallery",
                        principalTable: "images",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_image_tags_tags_tags_id",
                        column: x => x.tags_id,
                        principalSchema: "gallery",
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_image_tags_tags_id",
                schema: "gallery",
                table: "image_tags",
                column: "tags_id");

            migrationBuilder.CreateIndex(
                name: "ix_tags_name",
                schema: "gallery",
                table: "tags",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "image_tags",
                schema: "gallery");

            migrationBuilder.DropTable(
                name: "images",
                schema: "gallery");

            migrationBuilder.DropTable(
                name: "tags",
                schema: "gallery");
        }
    }
}
