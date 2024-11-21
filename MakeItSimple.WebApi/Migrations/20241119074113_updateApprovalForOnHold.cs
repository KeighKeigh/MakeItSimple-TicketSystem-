using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class updateApprovalForOnHold : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_reject_on_hold",
                table: "ticket_on_holds",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "on_hold_remarks",
                table: "ticket_on_holds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "reject_on_hold_at",
                table: "ticket_on_holds",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "reject_on_hold_by",
                table: "ticket_on_holds",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reject_remarks",
                table: "ticket_on_holds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ticket_approver",
                table: "ticket_on_holds",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ticket_on_hold_id",
                table: "approver_ticketings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_ticket_on_holds_reject_on_hold_by",
                table: "ticket_on_holds",
                column: "reject_on_hold_by");

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_ticket_on_hold_id",
                table: "approver_ticketings",
                column: "ticket_on_hold_id");

            migrationBuilder.AddForeignKey(
                name: "fk_approver_ticketings_ticket_on_holds_ticket_on_hold_id",
                table: "approver_ticketings",
                column: "ticket_on_hold_id",
                principalTable: "ticket_on_holds",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_on_holds_users_reject_on_hold_by_user_id",
                table: "ticket_on_holds",
                column: "reject_on_hold_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_approver_ticketings_ticket_on_holds_ticket_on_hold_id",
                table: "approver_ticketings");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_on_holds_users_reject_on_hold_by_user_id",
                table: "ticket_on_holds");

            migrationBuilder.DropIndex(
                name: "ix_ticket_on_holds_reject_on_hold_by",
                table: "ticket_on_holds");

            migrationBuilder.DropIndex(
                name: "ix_approver_ticketings_ticket_on_hold_id",
                table: "approver_ticketings");

            migrationBuilder.DropColumn(
                name: "is_reject_on_hold",
                table: "ticket_on_holds");

            migrationBuilder.DropColumn(
                name: "on_hold_remarks",
                table: "ticket_on_holds");

            migrationBuilder.DropColumn(
                name: "reject_on_hold_at",
                table: "ticket_on_holds");

            migrationBuilder.DropColumn(
                name: "reject_on_hold_by",
                table: "ticket_on_holds");

            migrationBuilder.DropColumn(
                name: "reject_remarks",
                table: "ticket_on_holds");

            migrationBuilder.DropColumn(
                name: "ticket_approver",
                table: "ticket_on_holds");

            migrationBuilder.DropColumn(
                name: "ticket_on_hold_id",
                table: "approver_ticketings");
        }
    }
}
