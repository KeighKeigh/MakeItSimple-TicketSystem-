using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class updateBackJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_back_job_id",
                table: "request_concerns",
                column: "back_job_id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_ticket_concerns_back_job_id1",
                table: "request_concerns",
                column: "back_job_id",
                principalTable: "ticket_concerns",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_ticket_concerns_back_job_id1",
                table: "request_concerns");

            migrationBuilder.DropIndex(
                name: "ix_request_concerns_back_job_id",
                table: "request_concerns");
        }
    }
}
