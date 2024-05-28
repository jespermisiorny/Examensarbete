using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Examensarbete.Migrations
{
    /// <inheritdoc />
    public partial class OnDeleteBehaviourChangeOnOrderData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderData_Products_ProductId",
                table: "OrderData");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderData_Products_ProductId",
                table: "OrderData",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderData_Products_ProductId",
                table: "OrderData");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderData_Products_ProductId",
                table: "OrderData",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
