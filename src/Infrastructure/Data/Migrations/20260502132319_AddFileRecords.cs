using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFileRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "file_records",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    original_file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    object_key = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    content_type = table.Column<string>(type: "character varying(127)", maxLength: 127, nullable: false),
                    size_bytes = table.Column<long>(type: "bigint", nullable: false),
                    category = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    created_date = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_date = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    updated_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_file_records", x => x.id);
                });

migrationBuilder.CreateIndex(
                name: "ix_file_records_category",
                table: "file_records",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "ix_file_records_created_date",
                table: "file_records",
                column: "created_date");

            migrationBuilder.CreateIndex(
                name: "ix_file_records_object_key",
                table: "file_records",
                column: "object_key",
                unique: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "file_records");

        }
    }
}
