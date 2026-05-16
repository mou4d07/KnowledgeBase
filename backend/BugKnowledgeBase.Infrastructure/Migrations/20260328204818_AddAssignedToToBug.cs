using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugKnowledgeBase.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedToToBug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedToSessionName",
                table: "Bugs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedToSessionName",
                table: "Bugs");
        }
    }
}
