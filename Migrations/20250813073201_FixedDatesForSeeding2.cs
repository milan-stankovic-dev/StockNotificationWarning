using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StockNotificationWarning.Migrations
{
    /// <inheritdoc />
    public partial class FixedDatesForSeeding2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProductDetails",
                keyColumn: "Id",
                keyValue: 5);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ProductDetails",
                columns: new[] { "Id", "CustomDescription", "CustomDisplayName", "ShopifyProductGid", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "This is a detailed description for the newest product.", "My Newest Product", "gid://shopify/Product/8858179666170", null },
                    { 2, "Comfortable sneakers for everyday use.", "Patike", "gid://shopify/Product/8858181271802", null },
                    { 3, "Premium boots for winter season.", "Cizme Model X", "gid://shopify/Product/8861054828794", null },
                    { 4, "Keep an eye on inventory for this one!", "My New Low Stock Product", "gid://shopify/Product/8861090283770", null },
                    { 5, "A classic design with modern comfort.", "Soko", "gid://shopify/Product/8861082714362", null }
                });
        }
    }
}
