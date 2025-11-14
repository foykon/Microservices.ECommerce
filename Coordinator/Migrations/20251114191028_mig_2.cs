using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Coordinator.Migrations
{
    /// <inheritdoc />
    public partial class mig_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Nodes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("5938cc94-1e6a-4039-a612-d7353c4cbe35"), "Stock.API" },
                    { new Guid("5a841ac9-71e6-49d8-9917-ed2d41959f2a"), "Payment.API" },
                    { new Guid("69e03bbc-fb6c-4b52-a12f-bf6da4bda7d8"), "Order.API" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("5938cc94-1e6a-4039-a612-d7353c4cbe35"));

            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("5a841ac9-71e6-49d8-9917-ed2d41959f2a"));

            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("69e03bbc-fb6c-4b52-a12f-bf6da4bda7d8"));
        }
    }
}
