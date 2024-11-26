using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC_Full.Migrations
{
    /// <inheritdoc />
    public partial class OrderingSystemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderID",
                table: "Tables",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tables_OrderID",
                table: "Tables",
                column: "OrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tables_Orders_OrderID",
                table: "Tables",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "OrderID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tables_Orders_OrderID",
                table: "Tables");

            migrationBuilder.DropIndex(
                name: "IX_Tables_OrderID",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "OrderID",
                table: "Tables");
        }
    }
}
