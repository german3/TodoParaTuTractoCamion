using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoParaTuTractoCamion.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDetallesAndCategoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "categoria",
                table: "Producto",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "detalles",
                table: "Producto",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "categoria",
                table: "Producto");

            migrationBuilder.DropColumn(
                name: "detalles",
                table: "Producto");
        }
    }
}
