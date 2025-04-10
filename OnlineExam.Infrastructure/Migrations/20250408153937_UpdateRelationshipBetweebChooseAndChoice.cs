using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineExam.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationshipBetweebChooseAndChoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Choices_ChooseQuestions_ChooseQuestionId",
                table: "Choices");

            migrationBuilder.AddForeignKey(
                name: "FK_Choices_ChooseQuestions_ChooseQuestionId",
                table: "Choices",
                column: "ChooseQuestionId",
                principalTable: "ChooseQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Choices_ChooseQuestions_ChooseQuestionId",
                table: "Choices");

            migrationBuilder.AddForeignKey(
                name: "FK_Choices_ChooseQuestions_ChooseQuestionId",
                table: "Choices",
                column: "ChooseQuestionId",
                principalTable: "ChooseQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
