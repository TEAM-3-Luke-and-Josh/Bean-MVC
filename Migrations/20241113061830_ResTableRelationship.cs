using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC_Full.Migrations
{
    /// <inheritdoc />
    public partial class ResTableRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReservationTables",
                columns: table => new
                {
                    ReservationsReservationID = table.Column<int>(type: "int", nullable: false),
                    TablesTableID = table.Column<string>(type: "nvarchar(4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationTables", x => new { x.ReservationsReservationID, x.TablesTableID });
                    table.ForeignKey(
                        name: "FK_ReservationTables_Reservations_ReservationsReservationID",
                        column: x => x.ReservationsReservationID,
                        principalTable: "Reservations",
                        principalColumn: "ReservationID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReservationTables_Tables_TablesTableID",
                        column: x => x.TablesTableID,
                        principalTable: "Tables",
                        principalColumn: "TableID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservationTables_TablesTableID",
                table: "ReservationTables",
                column: "TablesTableID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservationTables");
        }
    }
}
