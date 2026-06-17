using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobTrackingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "AuditLogs");
        }
    }
}
