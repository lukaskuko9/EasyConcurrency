#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

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
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    LockedUntil = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyDbEntities", x => x.Id);
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MyDbEntities");
        }
    }
}
