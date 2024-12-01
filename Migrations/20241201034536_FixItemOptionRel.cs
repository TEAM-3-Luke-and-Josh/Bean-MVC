using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeanScene.Migrations
{
    /// <inheritdoc />
    public partial class FixItemOptionRel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemOptions_MenuItems_MenuItemItemID",
                table: "ItemOptions");

            migrationBuilder.DropIndex(
                name: "IX_ItemOptions_MenuItemItemID",
                table: "ItemOptions");

            migrationBuilder.DropColumn(
                name: "MenuItemItemID",
                table: "ItemOptions");

            migrationBuilder.CreateIndex(
                name: "IX_ItemOptions_ItemID",
                table: "ItemOptions",
                column: "ItemID");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOptions_MenuItems_ItemID",
                table: "ItemOptions",
                column: "ItemID",
                principalTable: "MenuItems",
                principalColumn: "ItemID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemOptions_MenuItems_ItemID",
                table: "ItemOptions");

            migrationBuilder.DropIndex(
                name: "IX_ItemOptions_ItemID",
                table: "ItemOptions");

            migrationBuilder.AddColumn<int>(
                name: "MenuItemItemID",
                table: "ItemOptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ItemOptions_MenuItemItemID",
                table: "ItemOptions",
                column: "MenuItemItemID");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOptions_MenuItems_MenuItemItemID",
                table: "ItemOptions",
                column: "MenuItemItemID",
                principalTable: "MenuItems",
                principalColumn: "ItemID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
