using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntegrationTests.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MyDbEntities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MyUniqueKey = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LockedUntil = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyDbEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MyLockableEntities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LockedUntil = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyLockableEntities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MyDbEntities_LockedUntil",
                table: "MyDbEntities",
                column: "LockedUntil");

            migrationBuilder.CreateIndex(
                name: "IX_MyDbEntities_MyUniqueKey",
                table: "MyDbEntities",
                column: "MyUniqueKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MyLockableEntities_LockedUntil",
                table: "MyLockableEntities",
                column: "LockedUntil");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MyDbEntities");

            migrationBuilder.DropTable(
                name: "MyLockableEntities");
        }
    }
}
