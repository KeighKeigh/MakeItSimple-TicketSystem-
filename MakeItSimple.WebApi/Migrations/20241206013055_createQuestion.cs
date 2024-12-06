using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class createQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pms_questionaire",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    question_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    added_by_user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by_user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pms_questionaire", x => x.id);
                    table.ForeignKey(
                        name: "fk_pms_questionaire_users_added_by_user_id",
                        column: x => x.added_by_user_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_pms_questionaire_users_modified_by_user_id",
                        column: x => x.modified_by_user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "question_transaction_id",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pms_questionaire_module_id = table.Column<int>(type: "int", nullable: false),
                    pms_question_id = table.Column<int>(type: "int", nullable: false),
                    pms_questionaire_id = table.Column<int>(type: "int", nullable: true),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_question_transaction_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_question_transaction_id_pms_questionaire_modules_pms_questionaire_module_id",
                        column: x => x.pms_questionaire_module_id,
                        principalTable: "pms_questionaire_modules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_question_transaction_id_pms_questionaire_pms_questionaire_id",
                        column: x => x.pms_questionaire_id,
                        principalTable: "pms_questionaire",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_pms_questionaire_added_by_user_id",
                table: "pms_questionaire",
                column: "added_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_pms_questionaire_modified_by_user_id",
                table: "pms_questionaire",
                column: "modified_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_question_transaction_id_pms_questionaire_id",
                table: "question_transaction_id",
                column: "pms_questionaire_id");

            migrationBuilder.CreateIndex(
                name: "ix_question_transaction_id_pms_questionaire_module_id",
                table: "question_transaction_id",
                column: "pms_questionaire_module_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "question_transaction_id");

            migrationBuilder.DropTable(
                name: "pms_questionaire");
        }
    }
}
