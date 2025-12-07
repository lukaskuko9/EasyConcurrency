using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntegrationTests.Migrations
{
    /// <inheritdoc />
    public partial class AddedPropsToMyLockableEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TestParameterGuid",
                table: "MyLockableEntities",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TestParameterString",
                table: "MyLockableEntities",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestParameterGuid",
                table: "MyLockableEntities");

            migrationBuilder.DropColumn(
                name: "TestParameterString",
                table: "MyLockableEntities");
        }
    }
}
