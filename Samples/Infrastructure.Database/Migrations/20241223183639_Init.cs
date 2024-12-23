using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SampleEntities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MyUuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    LockedUntil = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleEntities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SampleEntities_IsProcessed_LockedUntil",
                table: "SampleEntities",
                columns: new[] { "IsProcessed", "LockedUntil" });

            migrationBuilder.CreateIndex(
                name: "IX_SampleEntities_MyUuid",
                table: "SampleEntities",
                column: "MyUuid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SampleEntities");
        }
    }
}
