using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class createQuestionTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pms_questionaire_users_added_by_user_id",
                table: "pms_questionaire");

            migrationBuilder.DropForeignKey(
                name: "fk_pms_questionaire_users_modified_by_user_id",
                table: "pms_questionaire");

            migrationBuilder.DropForeignKey(
                name: "fk_question_transaction_id_pms_questionaire_pms_questionaire_id",
                table: "question_transaction_id");

            migrationBuilder.DropPrimaryKey(
                name: "pk_pms_questionaire",
                table: "pms_questionaire");

            migrationBuilder.DropIndex(
                name: "ix_pms_questionaire_added_by_user_id",
                table: "pms_questionaire");

            migrationBuilder.DropIndex(
                name: "ix_pms_questionaire_modified_by_user_id",
                table: "pms_questionaire");

            migrationBuilder.DropColumn(
                name: "added_by_user_id",
                table: "pms_questionaire");

            migrationBuilder.DropColumn(
                name: "modified_by_user_id",
                table: "pms_questionaire");

            migrationBuilder.RenameTable(
                name: "pms_questionaire",
                newName: "pms_questionaires");

            migrationBuilder.AddPrimaryKey(
                name: "pk_pms_questionaires",
                table: "pms_questionaires",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_pms_questionaires_added_by",
                table: "pms_questionaires",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_pms_questionaires_modified_by",
                table: "pms_questionaires",
                column: "modified_by");

            migrationBuilder.AddForeignKey(
                name: "fk_pms_questionaires_users_added_by_user_id",
                table: "pms_questionaires",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_pms_questionaires_users_modified_by_user_id",
                table: "pms_questionaires",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_question_transaction_id_pms_questionaires_pms_questionaire_id",
                table: "question_transaction_id",
                column: "pms_questionaire_id",
                principalTable: "pms_questionaires",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pms_questionaires_users_added_by_user_id",
                table: "pms_questionaires");

            migrationBuilder.DropForeignKey(
                name: "fk_pms_questionaires_users_modified_by_user_id",
                table: "pms_questionaires");

            migrationBuilder.DropForeignKey(
                name: "fk_question_transaction_id_pms_questionaires_pms_questionaire_id",
                table: "question_transaction_id");

            migrationBuilder.DropPrimaryKey(
                name: "pk_pms_questionaires",
                table: "pms_questionaires");

            migrationBuilder.DropIndex(
                name: "ix_pms_questionaires_added_by",
                table: "pms_questionaires");

            migrationBuilder.DropIndex(
                name: "ix_pms_questionaires_modified_by",
                table: "pms_questionaires");

            migrationBuilder.RenameTable(
                name: "pms_questionaires",
                newName: "pms_questionaire");

            migrationBuilder.AddColumn<Guid>(
                name: "added_by_user_id",
                table: "pms_questionaire",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "modified_by_user_id",
                table: "pms_questionaire",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_pms_questionaire",
                table: "pms_questionaire",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_pms_questionaire_added_by_user_id",
                table: "pms_questionaire",
                column: "added_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_pms_questionaire_modified_by_user_id",
                table: "pms_questionaire",
                column: "modified_by_user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_pms_questionaire_users_added_by_user_id",
                table: "pms_questionaire",
                column: "added_by_user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_pms_questionaire_users_modified_by_user_id",
                table: "pms_questionaire",
                column: "modified_by_user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_question_transaction_id_pms_questionaire_pms_questionaire_id",
                table: "question_transaction_id",
                column: "pms_questionaire_id",
                principalTable: "pms_questionaire",
                principalColumn: "id");
        }
    }
}
