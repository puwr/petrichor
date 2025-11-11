using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Petrichor.Services.Gallery.Common.Persistence.Migrations
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
                    original_image_path = table.Column<string>(type: "text", nullable: false),
                    original_image_width = table.Column<int>(type: "integer", nullable: false),
                    original_image_height = table.Column<int>(type: "integer", nullable: false),
                    thumbnail_path = table.Column<string>(type: "text", nullable: false),
                    thumbnail_width = table.Column<int>(type: "integer", nullable: false),
                    thumbnail_height = table.Column<int>(type: "integer", nullable: false),
                    uploader_id = table.Column<Guid>(type: "uuid", nullable: false),
                    uploaded_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_images", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "inbox_message_consumers",
                schema: "gallery",
                columns: table => new
                {
                    inbox_message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inbox_message_consumers", x => new { x.inbox_message_id, x.name });
                });

            migrationBuilder.CreateTable(
                name: "inbox_messages",
                schema: "gallery",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "jsonb", maxLength: 2000, nullable: false),
                    occurred_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inbox_messages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "outbox_message_consumers",
                schema: "gallery",
                columns: table => new
                {
                    outbox_message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_message_consumers", x => new { x.outbox_message_id, x.name });
                });

            migrationBuilder.CreateTable(
                name: "outbox_messages",
                schema: "gallery",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "jsonb", maxLength: 2000, nullable: false),
                    occurred_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_messages", x => x.id);
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
                name: "user_snapshots",
                schema: "gallery",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_snapshots", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "image_tags",
                schema: "gallery",
                columns: table => new
                {
                    image_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tag_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_image_tags", x => new { x.image_id, x.tag_id });
                    table.ForeignKey(
                        name: "fk_image_tags_images_image_id",
                        column: x => x.image_id,
                        principalSchema: "gallery",
                        principalTable: "images",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_image_tags_tags_tag_id",
                        column: x => x.tag_id,
                        principalSchema: "gallery",
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_image_tags_tag_id_image_id",
                schema: "gallery",
                table: "image_tags",
                columns: new[] { "tag_id", "image_id" });

            migrationBuilder.CreateIndex(
                name: "ix_images_id",
                schema: "gallery",
                table: "images",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_images_uploader_id",
                schema: "gallery",
                table: "images",
                column: "uploader_id");

            migrationBuilder.CreateIndex(
                name: "ix_tags_id",
                schema: "gallery",
                table: "tags",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tags_name",
                schema: "gallery",
                table: "tags",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_snapshots_user_id",
                schema: "gallery",
                table: "user_snapshots",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_snapshots_user_name",
                schema: "gallery",
                table: "user_snapshots",
                column: "user_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "image_tags",
                schema: "gallery");

            migrationBuilder.DropTable(
                name: "inbox_message_consumers",
                schema: "gallery");

            migrationBuilder.DropTable(
                name: "inbox_messages",
                schema: "gallery");

            migrationBuilder.DropTable(
                name: "outbox_message_consumers",
                schema: "gallery");

            migrationBuilder.DropTable(
                name: "outbox_messages",
                schema: "gallery");

            migrationBuilder.DropTable(
                name: "user_snapshots",
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
