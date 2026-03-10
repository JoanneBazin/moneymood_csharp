using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moneymood.Migrations
{
    /// <inheritdoc />
    public partial class FixSpecialCategoryFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_SpecialCategories_SpecialBudgetId",
                table: "Expenses");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_SpecialCategoryId",
                table: "Expenses",
                column: "SpecialCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_SpecialCategories_SpecialCategoryId",
                table: "Expenses",
                column: "SpecialCategoryId",
                principalTable: "SpecialCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_SpecialCategories_SpecialCategoryId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_SpecialCategoryId",
                table: "Expenses");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_SpecialCategories_SpecialBudgetId",
                table: "Expenses",
                column: "SpecialBudgetId",
                principalTable: "SpecialCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
