using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class createFormQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_question_category_forms_form_id",
                table: "question_category");

            migrationBuilder.DropForeignKey(
                name: "fk_question_category_users_added_by_user_id",
                table: "question_category");

            migrationBuilder.DropForeignKey(
                name: "fk_question_category_users_modified_by_user_id",
                table: "question_category");

            migrationBuilder.DropPrimaryKey(
                name: "pk_question_category",
                table: "question_category");

            migrationBuilder.DropIndex(
                name: "ix_question_category_added_by_user_id",
                table: "question_category");

            migrationBuilder.DropIndex(
                name: "ix_question_category_modified_by_user_id",
                table: "question_category");

            migrationBuilder.DropColumn(
                name: "added_by_user_id",
                table: "question_category");

            migrationBuilder.DropColumn(
                name: "modified_by_user_id",
                table: "question_category");

            migrationBuilder.RenameTable(
                name: "question_category",
                newName: "question_categories");

            migrationBuilder.RenameIndex(
                name: "ix_question_category_form_id",
                table: "question_categories",
                newName: "ix_question_categories_form_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_question_categories",
                table: "question_categories",
                column: "id");

            migrationBuilder.CreateTable(
                name: "form_questions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    question_category_id = table.Column<int>(type: "int", nullable: false),
                    question_type = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_form_questions", x => x.id);
                    table.ForeignKey(
                        name: "fk_form_questions_question_categories_question_category_id",
                        column: x => x.question_category_id,
                        principalTable: "question_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_form_questions_users_added_by_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_form_questions_users_modified_by_user_id",
                        column: x => x.modified_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "form_check_boxes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    check_box_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    form_question_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_form_check_boxes", x => x.id);
                    table.ForeignKey(
                        name: "fk_form_check_boxes_form_questions_form_question_id",
                        column: x => x.form_question_id,
                        principalTable: "form_questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_form_check_boxes_users_added_by_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_form_check_boxes_users_modified_by_user_id",
                        column: x => x.modified_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "form_dropdowns",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    dropdown_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    form_question_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_form_dropdowns", x => x.id);
                    table.ForeignKey(
                        name: "fk_form_dropdowns_form_questions_form_question_id",
                        column: x => x.form_question_id,
                        principalTable: "form_questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_form_dropdowns_users_added_by_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_form_dropdowns_users_modified_by_user_id",
                        column: x => x.modified_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_question_categories_added_by",
                table: "question_categories",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_question_categories_modified_by",
                table: "question_categories",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_form_check_boxes_added_by",
                table: "form_check_boxes",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_form_check_boxes_form_question_id",
                table: "form_check_boxes",
                column: "form_question_id");

            migrationBuilder.CreateIndex(
                name: "ix_form_check_boxes_modified_by",
                table: "form_check_boxes",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_form_dropdowns_added_by",
                table: "form_dropdowns",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_form_dropdowns_form_question_id",
                table: "form_dropdowns",
                column: "form_question_id");

            migrationBuilder.CreateIndex(
                name: "ix_form_dropdowns_modified_by",
                table: "form_dropdowns",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_form_questions_added_by",
                table: "form_questions",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_form_questions_modified_by",
                table: "form_questions",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_form_questions_question_category_id",
                table: "form_questions",
                column: "question_category_id");

            migrationBuilder.AddForeignKey(
                name: "fk_question_categories_forms_form_id",
                table: "question_categories",
                column: "form_id",
                principalTable: "forms",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_question_categories_users_added_by_user_id",
                table: "question_categories",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_question_categories_users_modified_by_user_id",
                table: "question_categories",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_question_categories_forms_form_id",
                table: "question_categories");

            migrationBuilder.DropForeignKey(
                name: "fk_question_categories_users_added_by_user_id",
                table: "question_categories");

            migrationBuilder.DropForeignKey(
                name: "fk_question_categories_users_modified_by_user_id",
                table: "question_categories");

            migrationBuilder.DropTable(
                name: "form_check_boxes");

            migrationBuilder.DropTable(
                name: "form_dropdowns");

            migrationBuilder.DropTable(
                name: "form_questions");

            migrationBuilder.DropPrimaryKey(
                name: "pk_question_categories",
                table: "question_categories");

            migrationBuilder.DropIndex(
                name: "ix_question_categories_added_by",
                table: "question_categories");

            migrationBuilder.DropIndex(
                name: "ix_question_categories_modified_by",
                table: "question_categories");

            migrationBuilder.RenameTable(
                name: "question_categories",
                newName: "question_category");

            migrationBuilder.RenameIndex(
                name: "ix_question_categories_form_id",
                table: "question_category",
                newName: "ix_question_category_form_id");

            migrationBuilder.AddColumn<Guid>(
                name: "added_by_user_id",
                table: "question_category",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "modified_by_user_id",
                table: "question_category",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_question_category",
                table: "question_category",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_question_category_added_by_user_id",
                table: "question_category",
                column: "added_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_question_category_modified_by_user_id",
                table: "question_category",
                column: "modified_by_user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_question_category_forms_form_id",
                table: "question_category",
                column: "form_id",
                principalTable: "forms",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_question_category_users_added_by_user_id",
                table: "question_category",
                column: "added_by_user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_question_category_users_modified_by_user_id",
                table: "question_category",
                column: "modified_by_user_id",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
