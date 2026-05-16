using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugKnowledgeBase.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClassifiedInformation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClassifiedInformation",
                table: "KnowledgeArticles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetStructure",
                table: "KnowledgeArticles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthorSessionName",
                table: "Bugs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClassifiedInformation",
                table: "Bugs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetStructure",
                table: "Bugs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassifiedInformation",
                table: "KnowledgeArticles");

            migrationBuilder.DropColumn(
                name: "TargetStructure",
                table: "KnowledgeArticles");

            migrationBuilder.DropColumn(
                name: "AuthorSessionName",
                table: "Bugs");

            migrationBuilder.DropColumn(
                name: "ClassifiedInformation",
                table: "Bugs");

            migrationBuilder.DropColumn(
                name: "TargetStructure",
                table: "Bugs");
        }
    }
}
