using ClosedXML.Excel;
using Microsoft.Data.SqlClient;
using System;
using System.IO;

// ====================================================================
// Importador de Excel → SQL Server para TractoCamionDB
// Lee el archivo productos.xlsx que está en la raíz del proyecto
// y carga los productos cuyo stock >= 0 a la tabla Productos.
// Si el producto ya existe (mismo nombre), lo omite.
// ====================================================================

string xlsxPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "productos.xlsx");
xlsxPath = Path.GetFullPath(xlsxPath);

if (!File.Exists(xlsxPath))
{
    // Intentar ruta alternativa directa
    xlsxPath = @"D:\Codrava\Proyecto\productos.xlsx";
}

Console.WriteLine($"Leyendo Excel desde: {xlsxPath}");

if (!File.Exists(xlsxPath))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("ERROR: No se encontró el archivo productos.xlsx");
    Console.ResetColor();
    return;
}

const string connectionString = "Server=Alien;Database=TractoCamionDB;Trusted_Connection=True;TrustServerCertificate=True;";

int importados = 0, omitidos = 0, errores = 0;

using var workbook = new XLWorkbook(xlsxPath);
var ws = workbook.Worksheet(1);
var rows = ws.RangeUsed().RowsUsed();

// Mostrar cabeceras para confirmar estructura
Console.WriteLine("\n=== Columnas detectadas en fila 1 (cabecera) ===");
for (int c = 1; c <= 15; c++)
{
    var h = ws.Cell(1, c).GetValue<string>();
    if (!string.IsNullOrWhiteSpace(h))
        Console.WriteLine($"  Col {c}: {h}");
}
Console.WriteLine();

int totalRows = 0;
foreach (var _ in rows) totalRows++;
Console.WriteLine($"Total filas (incluyendo header): {totalRows}");
Console.WriteLine("Iniciando importación...\n");

using var conn = new SqlConnection(connectionString);
conn.Open();

foreach (var row in rows)
{
    if (row.RowNumber() == 1) { } // El Excel no tiene fila de encabezado, los datos empiezan en fila 1


    try
    {
        string nombre = row.Cell(1).GetValue<string>()?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(nombre)) continue;

        // Intentar leer precio de col B
        decimal precio = 0;
        var precioCell = row.Cell(2);
        try { precio = precioCell.GetValue<decimal>(); } catch { }
        if (precio <= 0) precio = 1;

        // Stock de col C (si existe), si no, usar 50 como default
        int stock = 0;
        try { stock = row.Cell(3).GetValue<int>(); } catch { stock = 0; }
        if (stock < 0) stock = 0;

        // Imágenes: columnas I(9), J(10), K(11)
        string img1 = row.Cell(9).GetValue<string>()?.Trim() ?? "";
        string img2 = row.Cell(10).GetValue<string>()?.Trim() ?? "";
        string img3 = row.Cell(11).GetValue<string>()?.Trim() ?? "";

        // Verificar si ya existe un producto con ese nombre
        using var checkCmd = new SqlCommand("SELECT COUNT(1) FROM Productos WHERE Nombre = @Nombre", conn);
        checkCmd.Parameters.AddWithValue("@Nombre", nombre);
        int existing = (int)checkCmd.ExecuteScalar();

        if (existing > 0)
        {
            omitidos++;
            continue;
        }

        // Insertar
        const string insertSql = @"
            INSERT INTO Productos (Id, Nombre, Precio, Stock, Imagen1Url, Imagen2Url, Imagen3Url, FechaCreacion)
            VALUES (@Id, @Nombre, @Precio, @Stock, @Img1, @Img2, @Img3, @FechaCreacion)";

        using var cmd = new SqlCommand(insertSql, conn);
        cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
        cmd.Parameters.AddWithValue("@Nombre", nombre);
        cmd.Parameters.AddWithValue("@Precio", precio);
        cmd.Parameters.AddWithValue("@Stock", stock);
        cmd.Parameters.AddWithValue("@Img1", img1);
        cmd.Parameters.AddWithValue("@Img2", img2);
        cmd.Parameters.AddWithValue("@Img3", img3);
        cmd.Parameters.AddWithValue("@FechaCreacion", DateTime.UtcNow);
        cmd.ExecuteNonQuery();

        importados++;

        if (importados % 50 == 0)
            Console.WriteLine($"  ✓ {importados} productos importados hasta ahora...");
    }
    catch (Exception ex)
    {
        errores++;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  ! Error en fila {row.RowNumber()}: {ex.Message}");
        Console.ResetColor();
    }
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"\n=== IMPORTACIÓN COMPLETADA ===");
Console.WriteLine($"  Importados : {importados}");
Console.WriteLine($"  Omitidos   : {omitidos} (ya existían)");
Console.WriteLine($"  Errores    : {errores}");
Console.ResetColor();
