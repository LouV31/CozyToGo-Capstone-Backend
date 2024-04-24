using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CozyToGo.Migrations
{
    /// <inheritdoc />
    public partial class editordersdelivers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDelivered",
                table: "Orders");

            migrationBuilder.AddColumn<bool>(
                name: "isDelivered",
                table: "OrderDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isDelivered",
                table: "OrderDetails");

            migrationBuilder.AddColumn<bool>(
                name: "IsDelivered",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
