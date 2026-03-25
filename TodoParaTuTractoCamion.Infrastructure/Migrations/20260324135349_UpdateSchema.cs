using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoParaTuTractoCamion.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Productos",
                table: "Productos");

            migrationBuilder.RenameTable(
                name: "Productos",
                newName: "Producto");

            migrationBuilder.RenameColumn(
                name: "Stock",
                table: "Producto",
                newName: "stock");

            migrationBuilder.RenameColumn(
                name: "Precio",
                table: "Producto",
                newName: "precio");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Producto",
                newName: "nombre");

            migrationBuilder.RenameColumn(
                name: "Imagen3Url",
                table: "Producto",
                newName: "imagen3Url");

            migrationBuilder.RenameColumn(
                name: "Imagen2Url",
                table: "Producto",
                newName: "imagen2Url");

            migrationBuilder.RenameColumn(
                name: "Imagen1Url",
                table: "Producto",
                newName: "imagen1Url");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Producto",
                newName: "id");

            migrationBuilder.AlterColumn<double>(
                name: "precio",
                table: "Producto",
                type: "double precision",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "imagen3Url",
                table: "Producto",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "imagen2Url",
                table: "Producto",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "imagen1Url",
                table: "Producto",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "Producto",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Producto",
                table: "Producto",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Producto",
                table: "Producto");

            migrationBuilder.RenameTable(
                name: "Producto",
                newName: "Productos");

            migrationBuilder.RenameColumn(
                name: "stock",
                table: "Productos",
                newName: "Stock");

            migrationBuilder.RenameColumn(
                name: "precio",
                table: "Productos",
                newName: "Precio");

            migrationBuilder.RenameColumn(
                name: "nombre",
                table: "Productos",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "imagen3Url",
                table: "Productos",
                newName: "Imagen3Url");

            migrationBuilder.RenameColumn(
                name: "imagen2Url",
                table: "Productos",
                newName: "Imagen2Url");

            migrationBuilder.RenameColumn(
                name: "imagen1Url",
                table: "Productos",
                newName: "Imagen1Url");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Productos",
                newName: "Id");

            migrationBuilder.AlterColumn<decimal>(
                name: "Precio",
                table: "Productos",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Imagen3Url",
                table: "Productos",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Imagen2Url",
                table: "Productos",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Imagen1Url",
                table: "Productos",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Productos",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Productos",
                table: "Productos",
                column: "Id");
        }
    }
}
