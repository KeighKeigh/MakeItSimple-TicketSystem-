using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class createPmsQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_question_transaction_id_pms_questionaire_modules_pms_questionaire_module_id",
                table: "question_transaction_id");

            migrationBuilder.DropForeignKey(
                name: "fk_question_transaction_id_pms_questionaires_pms_questionaire_id",
                table: "question_transaction_id");

            migrationBuilder.DropPrimaryKey(
                name: "pk_question_transaction_id",
                table: "question_transaction_id");

            migrationBuilder.RenameTable(
                name: "question_transaction_id",
                newName: "question_transaction_ids");

            migrationBuilder.RenameIndex(
                name: "ix_question_transaction_id_pms_questionaire_module_id",
                table: "question_transaction_ids",
                newName: "ix_question_transaction_ids_pms_questionaire_module_id");

            migrationBuilder.RenameIndex(
                name: "ix_question_transaction_id_pms_questionaire_id",
                table: "question_transaction_ids",
                newName: "ix_question_transaction_ids_pms_questionaire_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_question_transaction_ids",
                table: "question_transaction_ids",
                column: "id");

            migrationBuilder.CreateTable(
                name: "pms_question_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pms_questionaire_id = table.Column<int>(type: "int", nullable: false),
                    question_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pms_question_types", x => x.id);
                    table.ForeignKey(
                        name: "fk_pms_question_types_pms_questionaires_pms_questionaire_id",
                        column: x => x.pms_questionaire_id,
                        principalTable: "pms_questionaires",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_pms_question_types_pms_questionaire_id",
                table: "pms_question_types",
                column: "pms_questionaire_id");

            migrationBuilder.AddForeignKey(
                name: "fk_question_transaction_ids_pms_questionaire_modules_pms_questionaire_module_id",
                table: "question_transaction_ids",
                column: "pms_questionaire_module_id",
                principalTable: "pms_questionaire_modules",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_question_transaction_ids_pms_questionaires_pms_questionaire_id",
                table: "question_transaction_ids",
                column: "pms_questionaire_id",
                principalTable: "pms_questionaires",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_question_transaction_ids_pms_questionaire_modules_pms_questionaire_module_id",
                table: "question_transaction_ids");

            migrationBuilder.DropForeignKey(
                name: "fk_question_transaction_ids_pms_questionaires_pms_questionaire_id",
                table: "question_transaction_ids");

            migrationBuilder.DropTable(
                name: "pms_question_types");

            migrationBuilder.DropPrimaryKey(
                name: "pk_question_transaction_ids",
                table: "question_transaction_ids");

            migrationBuilder.RenameTable(
                name: "question_transaction_ids",
                newName: "question_transaction_id");

            migrationBuilder.RenameIndex(
                name: "ix_question_transaction_ids_pms_questionaire_module_id",
                table: "question_transaction_id",
                newName: "ix_question_transaction_id_pms_questionaire_module_id");

            migrationBuilder.RenameIndex(
                name: "ix_question_transaction_ids_pms_questionaire_id",
                table: "question_transaction_id",
                newName: "ix_question_transaction_id_pms_questionaire_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_question_transaction_id",
                table: "question_transaction_id",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_question_transaction_id_pms_questionaire_modules_pms_questionaire_module_id",
                table: "question_transaction_id",
                column: "pms_questionaire_module_id",
                principalTable: "pms_questionaire_modules",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_question_transaction_id_pms_questionaires_pms_questionaire_id",
                table: "question_transaction_id",
                column: "pms_questionaire_id",
                principalTable: "pms_questionaires",
                principalColumn: "id");
        }
    }
}
