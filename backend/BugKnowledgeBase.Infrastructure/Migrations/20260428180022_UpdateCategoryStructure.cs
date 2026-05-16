using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugKnowledgeBase.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCategoryStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bugs_BugCategories_CategoryId",
                table: "Bugs");

            migrationBuilder.DropForeignKey(
                name: "FK_KnowledgeArticles_BugCategories_CategoryId",
                table: "KnowledgeArticles");

            migrationBuilder.DropForeignKey(
                name: "FK_BugCategories_BugCategories_ParentCategoryId",
                table: "BugCategories");

            migrationBuilder.RenameTable(
                name: "BugCategories",
                newName: "Categories");

            migrationBuilder.RenameIndex(
                name: "IX_BugCategories_ParentCategoryId",
                table: "Categories",
                newName: "IX_Categories_ParentCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bugs_Categories_CategoryId",
                table: "Bugs",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KnowledgeArticles_Categories_CategoryId",
                table: "KnowledgeArticles",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bugs_Categories_CategoryId",
                table: "Bugs");

            migrationBuilder.DropForeignKey(
                name: "FK_KnowledgeArticles_Categories_CategoryId",
                table: "KnowledgeArticles");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_ParentCategoryId",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "BugCategories");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "BugCategories",
                newName: "IX_BugCategories_ParentCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_BugCategories_BugCategories_ParentCategoryId",
                table: "BugCategories",
                column: "ParentCategoryId",
                principalTable: "BugCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bugs_BugCategories_CategoryId",
                table: "Bugs",
                column: "CategoryId",
                principalTable: "BugCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KnowledgeArticles_BugCategories_CategoryId",
                table: "KnowledgeArticles",
                column: "CategoryId",
                principalTable: "BugCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
