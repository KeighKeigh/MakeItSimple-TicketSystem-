using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class updateQuestionTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_question_transaction_ids_pms_questionaires_pms_questionaire_id",
                table: "question_transaction_ids");

            migrationBuilder.DropColumn(
                name: "pms_question_id",
                table: "question_transaction_ids");

            migrationBuilder.AlterColumn<int>(
                name: "pms_questionaire_id",
                table: "question_transaction_ids",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_question_transaction_ids_pms_questionaires_pms_questionaire_id",
                table: "question_transaction_ids",
                column: "pms_questionaire_id",
                principalTable: "pms_questionaires",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_question_transaction_ids_pms_questionaires_pms_questionaire_id",
                table: "question_transaction_ids");

            migrationBuilder.AlterColumn<int>(
                name: "pms_questionaire_id",
                table: "question_transaction_ids",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "pms_question_id",
                table: "question_transaction_ids",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "fk_question_transaction_ids_pms_questionaires_pms_questionaire_id",
                table: "question_transaction_ids",
                column: "pms_questionaire_id",
                principalTable: "pms_questionaires",
                principalColumn: "id");
        }
    }
}
