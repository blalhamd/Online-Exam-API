using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineExam.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChooseQuestions_Exams_ExamId",
                table: "ChooseQuestions");

            migrationBuilder.AddForeignKey(
                name: "FK_ChooseQuestions_Exams_ExamId",
                table: "ChooseQuestions",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChooseQuestions_Exams_ExamId",
                table: "ChooseQuestions");

            migrationBuilder.AddForeignKey(
                name: "FK_ChooseQuestions_Exams_ExamId",
                table: "ChooseQuestions",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
