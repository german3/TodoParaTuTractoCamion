using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoParaTuTractoCamion.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Migracion_893_Productos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    precio = table.Column<double>(type: "double precision", precision: 18, scale: 2, nullable: false),
                    stock = table.Column<int>(type: "integer", nullable: false),
                    imagen1Url = table.Column<string>(type: "text", nullable: true),
                    imagen2Url = table.Column<string>(type: "text", nullable: true),
                    imagen3Url = table.Column<string>(type: "text", nullable: true),
                    detalles = table.Column<string>(type: "text", nullable: true),
                    categoria = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Producto");
        }
    }
}
