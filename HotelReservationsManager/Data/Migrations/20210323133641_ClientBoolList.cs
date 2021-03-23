using Microsoft.EntityFrameworkCore.Migrations;

namespace HotelReservationsManager.Data.Migrations
{
    public partial class ClientBoolList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Client_Reservation_ReservationID",
                table: "Client");

            migrationBuilder.DropIndex(
                name: "IX_Client_ReservationID",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "ReservationID",
                table: "Client");

            migrationBuilder.AddColumn<bool>(
                name: "bCurrInReservation",
                table: "Client",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ClientReservation",
                columns: table => new
                {
                    clientsID = table.Column<int>(type: "int", nullable: false),
                    reservationsID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientReservation", x => new { x.clientsID, x.reservationsID });
                    table.ForeignKey(
                        name: "FK_ClientReservation_Client_clientsID",
                        column: x => x.clientsID,
                        principalTable: "Client",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientReservation_Reservation_reservationsID",
                        column: x => x.reservationsID,
                        principalTable: "Reservation",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientReservation_reservationsID",
                table: "ClientReservation",
                column: "reservationsID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientReservation");

            migrationBuilder.DropColumn(
                name: "bCurrInReservation",
                table: "Client");

            migrationBuilder.AddColumn<int>(
                name: "ReservationID",
                table: "Client",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Client_ReservationID",
                table: "Client",
                column: "ReservationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Client_Reservation_ReservationID",
                table: "Client",
                column: "ReservationID",
                principalTable: "Reservation",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
