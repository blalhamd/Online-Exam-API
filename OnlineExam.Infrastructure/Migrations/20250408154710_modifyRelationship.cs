using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineExam.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class modifyRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_Choices_SelectedChoiceId",
                table: "UserAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_ChooseQuestions_ChooseQuestionId",
                table: "UserAnswers");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_Choices_SelectedChoiceId",
                table: "UserAnswers",
                column: "SelectedChoiceId",
                principalTable: "Choices",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_ChooseQuestions_ChooseQuestionId",
                table: "UserAnswers",
                column: "ChooseQuestionId",
                principalTable: "ChooseQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_Choices_SelectedChoiceId",
                table: "UserAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_ChooseQuestions_ChooseQuestionId",
                table: "UserAnswers");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_Choices_SelectedChoiceId",
                table: "UserAnswers",
                column: "SelectedChoiceId",
                principalTable: "Choices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_ChooseQuestions_ChooseQuestionId",
                table: "UserAnswers",
                column: "ChooseQuestionId",
                principalTable: "ChooseQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
