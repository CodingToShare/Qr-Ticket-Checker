using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qr_Ticket_Checker.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUpdateModelPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TemplateIdentifier",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemplateIdentifier",
                table: "Events");
        }
    }
}
