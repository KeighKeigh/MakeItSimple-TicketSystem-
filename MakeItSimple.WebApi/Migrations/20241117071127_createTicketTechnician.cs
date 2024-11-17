using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class createTicketTechnician : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ticket_technicians",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    closing_ticket_id = table.Column<int>(type: "int", nullable: false),
                    technician_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ticket_technicians", x => x.id);
                    table.ForeignKey(
                        name: "fk_ticket_technicians_closing_tickets_closing_ticket_id",
                        column: x => x.closing_ticket_id,
                        principalTable: "closing_tickets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_ticket_technicians_users_technician_by_user_id",
                        column: x => x.technician_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ticket_technicians_closing_ticket_id",
                table: "ticket_technicians",
                column: "closing_ticket_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_technicians_technician_by",
                table: "ticket_technicians",
                column: "technician_by");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ticket_technicians");
        }
    }
}
