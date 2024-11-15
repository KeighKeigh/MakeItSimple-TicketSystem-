using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class createTicketCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ticket_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    request_concern_id = table.Column<int>(type: "int", nullable: false),
                    category_id = table.Column<int>(type: "int", nullable: false),
                    is_removed = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ticket_categories", x => x.id);
                    table.ForeignKey(
                        name: "fk_ticket_categories_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ticket_categories_request_concerns_request_concern_id",
                        column: x => x.request_concern_id,
                        principalTable: "request_concerns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ticket_sub_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    request_concern_id = table.Column<int>(type: "int", nullable: false),
                    sub_category_id = table.Column<int>(type: "int", nullable: false),
                    is_removed = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ticket_sub_categories", x => x.id);
                    table.ForeignKey(
                        name: "fk_ticket_sub_categories_request_concerns_request_concern_id",
                        column: x => x.request_concern_id,
                        principalTable: "request_concerns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ticket_sub_categories_sub_categories_sub_category_id",
                        column: x => x.sub_category_id,
                        principalTable: "sub_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ticket_categories_category_id",
                table: "ticket_categories",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_categories_request_concern_id",
                table: "ticket_categories",
                column: "request_concern_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_sub_categories_request_concern_id",
                table: "ticket_sub_categories",
                column: "request_concern_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_sub_categories_sub_category_id",
                table: "ticket_sub_categories",
                column: "sub_category_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ticket_categories");

            migrationBuilder.DropTable(
                name: "ticket_sub_categories");
        }
    }
}
