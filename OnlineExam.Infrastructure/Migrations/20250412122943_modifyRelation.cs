using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineExam.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class modifyRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_Choices_SelectedChoiceId",
                table: "UserAnswers");

            migrationBuilder.DropIndex(
                name: "IX_UserAnswers_SelectedChoiceId",
                table: "UserAnswers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_SelectedChoiceId",
                table: "UserAnswers",
                column: "SelectedChoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_Choices_SelectedChoiceId",
                table: "UserAnswers",
                column: "SelectedChoiceId",
                principalTable: "Choices",
                principalColumn: "Id");
        }
    }
}
