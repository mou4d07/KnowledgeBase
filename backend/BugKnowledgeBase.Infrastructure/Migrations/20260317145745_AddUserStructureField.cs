using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugKnowledgeBase.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserStructureField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Structure",
                table: "AuthorizedUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Structure",
                table: "AuthorizedUsers");
        }
    }
}
