using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Petrichor.Services.Comments.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "comments");

            migrationBuilder.CreateTable(
                name: "comments",
                schema: "comments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    resource_id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_comments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_snapshots",
                schema: "comments",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_snapshots", x => x.user_id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_comments_resource_id",
                schema: "comments",
                table: "comments",
                column: "resource_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_snapshots_user_name",
                schema: "comments",
                table: "user_snapshots",
                column: "user_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comments",
                schema: "comments");

            migrationBuilder.DropTable(
                name: "user_snapshots",
                schema: "comments");
        }
    }
}
