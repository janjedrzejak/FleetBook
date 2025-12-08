using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetBook.API.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationStatusAndFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataZwrotu",
                table: "Reservations",
                newName: "ApprovedAt");

            migrationBuilder.RenameColumn(
                name: "DataRezerwacji",
                table: "Reservations",
                newName: "NotatkiAkceptujacego");

            migrationBuilder.AddColumn<int>(
                name: "ApprovedByUserId",
                table: "Reservations",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Reservations",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataDo",
                table: "Reservations",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataOd",
                table: "Reservations",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Notatki",
                table: "Reservations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Reservations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ApprovedByUserId",
                table: "Reservations",
                column: "ApprovedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Users_ApprovedByUserId",
                table: "Reservations",
                column: "ApprovedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Users_ApprovedByUserId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_ApprovedByUserId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "DataDo",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "DataOd",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Notatki",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "NotatkiAkceptujacego",
                table: "Reservations",
                newName: "DataRezerwacji");

            migrationBuilder.RenameColumn(
                name: "ApprovedAt",
                table: "Reservations",
                newName: "DataZwrotu");
        }
    }
}
