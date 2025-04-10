using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineExam.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Last : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_Choices_SelectedChoiceId",
                table: "UserAnswers");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_Choices_SelectedChoiceId",
                table: "UserAnswers",
                column: "SelectedChoiceId",
                principalTable: "Choices",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_Choices_SelectedChoiceId",
                table: "UserAnswers");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_Choices_SelectedChoiceId",
                table: "UserAnswers",
                column: "SelectedChoiceId",
                principalTable: "Choices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
