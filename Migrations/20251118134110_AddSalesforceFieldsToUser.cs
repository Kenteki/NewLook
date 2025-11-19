using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewLook.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesforceFieldsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SalesforceAccountId",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalesforceContactId",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalesforceAccountId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SalesforceContactId",
                table: "Users");
        }
    }
}
