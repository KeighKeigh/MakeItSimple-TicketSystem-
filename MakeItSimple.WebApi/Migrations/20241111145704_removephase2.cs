using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class removephase2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "question_module_forms");

            migrationBuilder.DropTable(
                name: "question_modules");

            migrationBuilder.CreateTable(
                name: "question_category",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    added_by_user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by_user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    form_id = table.Column<int>(type: "int", nullable: false),
                    question_category_name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_question_category", x => x.id);
                    table.ForeignKey(
                        name: "fk_question_category_forms_form_id",
                        column: x => x.form_id,
                        principalTable: "forms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_question_category_users_added_by_user_id",
                        column: x => x.added_by_user_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_question_category_users_modified_by_user_id",
                        column: x => x.modified_by_user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_question_category_added_by_user_id",
                table: "question_category",
                column: "added_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_question_category_form_id",
                table: "question_category",
                column: "form_id");

            migrationBuilder.CreateIndex(
                name: "ix_question_category_modified_by_user_id",
                table: "question_category",
                column: "modified_by_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "question_category");

            migrationBuilder.CreateTable(
                name: "question_modules",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    question_modules_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_question_modules", x => x.id);
                    table.ForeignKey(
                        name: "fk_question_modules_users_added_by_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_question_modules_users_modified_by_user_id",
                        column: x => x.modified_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "question_module_forms",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    form_id = table.Column<int>(type: "int", nullable: false),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    question_module_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_question_module_forms", x => x.id);
                    table.ForeignKey(
                        name: "fk_question_module_forms_forms_form_id",
                        column: x => x.form_id,
                        principalTable: "forms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_question_module_forms_question_modules_question_module_id",
                        column: x => x.question_module_id,
                        principalTable: "question_modules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_question_module_forms_users_added_by_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_question_module_forms_users_modified_by_user_id",
                        column: x => x.modified_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_question_module_forms_added_by",
                table: "question_module_forms",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_question_module_forms_form_id",
                table: "question_module_forms",
                column: "form_id");

            migrationBuilder.CreateIndex(
                name: "ix_question_module_forms_modified_by",
                table: "question_module_forms",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_question_module_forms_question_module_id",
                table: "question_module_forms",
                column: "question_module_id");

            migrationBuilder.CreateIndex(
                name: "ix_question_modules_added_by",
                table: "question_modules",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_question_modules_modified_by",
                table: "question_modules",
                column: "modified_by");
        }
    }
}
