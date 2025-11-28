using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetBook.Migrations
{
    /// <inheritdoc />
    public partial class AddUprawniony : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Uprawniony",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Uprawniony",
                table: "Users");
        }
    }
}
