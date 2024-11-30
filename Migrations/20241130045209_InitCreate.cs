using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeanScene.Migrations
{
    /// <inheritdoc />
    public partial class InitCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Guests",
                columns: table => new
                {
                    GuestID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guests", x => x.GuestID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MenuCategories",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsAvailable = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuCategories", x => x.CategoryID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Sittings",
                columns: table => new
                {
                    SittingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SittingType = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StartTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    ClosedForReservations = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sittings", x => x.SittingID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "varchar(255)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserType = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    ItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsAvailable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PrepTime = table.Column<int>(type: "int", nullable: true),
                    ImageUrl = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.ItemID);
                    table.ForeignKey(
                        name: "FK_MenuItems_MenuCategories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "MenuCategories",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    ReservationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GuestID = table.Column<int>(type: "int", nullable: false),
                    SittingID = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    NumberOfGuests = table.Column<int>(type: "int", nullable: false),
                    ReservationStatus = table.Column<string>(type: "varchar(50)", nullable: false, defaultValue: "Pending")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.ReservationID);
                    table.ForeignKey(
                        name: "FK_Reservations_Guests_GuestID",
                        column: x => x.GuestID,
                        principalTable: "Guests",
                        principalColumn: "GuestID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservations_Sittings_SittingID",
                        column: x => x.SittingID,
                        principalTable: "Sittings",
                        principalColumn: "SittingID",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemOptions",
                columns: table => new
                {
                    OptionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ItemID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PriceModifier = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    MenuItemItemID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemOptions", x => x.OptionID);
                    table.ForeignKey(
                        name: "FK_ItemOptions_MenuItems_MenuItemItemID",
                        column: x => x.MenuItemItemID,
                        principalTable: "MenuItems",
                        principalColumn: "ItemID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MenuAvailability",
                columns: table => new
                {
                    ItemID = table.Column<int>(type: "int", nullable: false),
                    SittingType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsAvailable = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuAvailability", x => new { x.ItemID, x.SittingType });
                    table.ForeignKey(
                        name: "FK_MenuAvailability_MenuItems_ItemID",
                        column: x => x.ItemID,
                        principalTable: "MenuItems",
                        principalColumn: "ItemID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrderItemOptions",
                columns: table => new
                {
                    OrderItemsOrderItemID = table.Column<int>(type: "int", nullable: false),
                    SelectedOptionsOptionID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemOptions", x => new { x.OrderItemsOrderItemID, x.SelectedOptionsOptionID });
                    table.ForeignKey(
                        name: "FK_OrderItemOptions_ItemOptions_SelectedOptionsOptionID",
                        column: x => x.SelectedOptionsOptionID,
                        principalTable: "ItemOptions",
                        principalColumn: "OptionID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    ItemID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SpecialInstructions = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ItemStatus = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MenuItemItemID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.OrderItemID);
                    table.ForeignKey(
                        name: "FK_OrderItems_MenuItems_MenuItemItemID",
                        column: x => x.MenuItemItemID,
                        principalTable: "MenuItems",
                        principalColumn: "ItemID",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ReservationID = table.Column<int>(type: "int", nullable: true),
                    TableID = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OrderStatus = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OrderTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    SpecialRequests = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_Orders_Reservations_ReservationID",
                        column: x => x.ReservationID,
                        principalTable: "Reservations",
                        principalColumn: "ReservationID",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tables",
                columns: table => new
                {
                    TableID = table.Column<string>(type: "varchar(4)", maxLength: 4, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Area = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    OrderID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.TableID);
                    table.ForeignKey(
                        name: "FK_Tables_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ReservationTables",
                columns: table => new
                {
                    ReservationsReservationID = table.Column<int>(type: "int", nullable: false),
                    TablesTableID = table.Column<string>(type: "varchar(4)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Guests_Email",
                table: "Guests",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guests_PhoneNumber",
                table: "Guests",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemOptions_MenuItemItemID",
                table: "ItemOptions",
                column: "MenuItemItemID");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_CategoryID",
                table: "MenuItems",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemOptions_SelectedOptionsOptionID",
                table: "OrderItemOptions",
                column: "SelectedOptionsOptionID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_MenuItemItemID",
                table: "OrderItems",
                column: "MenuItemItemID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderID",
                table: "OrderItems",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ReservationID",
                table: "Orders",
                column: "ReservationID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TableID",
                table: "Orders",
                column: "TableID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_GuestID",
                table: "Reservations",
                column: "GuestID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_SittingID",
                table: "Reservations",
                column: "SittingID");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationTables_TablesTableID",
                table: "ReservationTables",
                column: "TablesTableID");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_OrderID",
                table: "Tables",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItemOptions_OrderItems_OrderItemsOrderItemID",
                table: "OrderItemOptions",
                column: "OrderItemsOrderItemID",
                principalTable: "OrderItems",
                principalColumn: "OrderItemID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderID",
                table: "OrderItems",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "OrderID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Tables_TableID",
                table: "Orders",
                column: "TableID",
                principalTable: "Tables",
                principalColumn: "TableID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tables_Orders_OrderID",
                table: "Tables");

            migrationBuilder.DropTable(
                name: "MenuAvailability");

            migrationBuilder.DropTable(
                name: "OrderItemOptions");

            migrationBuilder.DropTable(
                name: "ReservationTables");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "ItemOptions");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DropTable(
                name: "MenuCategories");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Tables");

            migrationBuilder.DropTable(
                name: "Guests");

            migrationBuilder.DropTable(
                name: "Sittings");
        }
    }
}
