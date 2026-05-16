using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugKnowledgeBase.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddArticleFeedbackFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Dislikes",
                table: "KnowledgeArticles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Likes",
                table: "KnowledgeArticles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dislikes",
                table: "KnowledgeArticles");

            migrationBuilder.DropColumn(
                name: "Likes",
                table: "KnowledgeArticles");
        }
    }
}
