using ExcelDataReader;
using Npgsql;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System;
using System.Data;
using System.IO;
using System.Text;

// ====================================================================
// Importador de Excel → PostgreSQL para TractoCamionDB (Usa ExcelDataReader)
// ====================================================================

// Registrar el proveedor de codificación para ExcelDataReader
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

string xlsxPath = @"D:\Codrava\Proyecto\Lista Productos.xlsx";

if (!File.Exists(xlsxPath))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("ERROR: No se encontró el archivo 'Lista Productos.xlsx'");
    Console.ResetColor();
    return;
}

Console.WriteLine($"Leyendo Excel desde: {xlsxPath}");

// Conexión a PostgreSQL (Se asume 'postgres' como password si 'tu_password' falla o viceversa)
const string connectionString = "Host=localhost;Database=TractoCamionDB;Username=postgres;Password=1234;";

int importados = 0, errores = 0;

try
{
    using var stream = File.Open(xlsxPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
    using var reader = ExcelReaderFactory.CreateReader(stream);

    Console.WriteLine("Iniciando actualización de base de datos...\n");

    // Pre-cargar Libro con EPPlus para imágenes
    Console.WriteLine("Cargando imágenes con EPPlus (esto puede tomar un momento)...");
    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    using var package = new ExcelPackage(new FileInfo(xlsxPath));
    var ws = package.Workbook.Worksheets[0]; // EPPlus es 0-based index para la colección de sheets
    var pictures = ws.Drawings.OfType<ExcelPicture>().ToList();
    Console.WriteLine($"Imágenes detectadas en el Excel: {pictures.Count}");

    string imagesDir = @"D:\Codrava\Proyecto\TodoParaTuTractoCamion.API\wwwroot\images\productos";
    Directory.CreateDirectory(imagesDir);

    using var conn = new NpgsqlConnection(connectionString);
    conn.Open();

    // 1. CONSULTAR DATOS (DEBUG)
    Console.WriteLine("Consultando los primeros 5 productos para verificar URLs...");
    using (var queryCmd = new NpgsqlCommand("SELECT nombre, \"imagen1Url\", stock FROM \"Producto\" LIMIT 5;", conn))
    {
        using var r = queryCmd.ExecuteReader();
        while (r.Read())
        {
            Console.WriteLine($"Producto: {r[0]}, Imagen: {r[1]}, Stock: {r[2]}");
        }
    }
    return;

    // 2. IMPORTAR REGISTROS
    // Helper para acceso seguro a columnas
    string GetSafeValue(IDataReader r, int idx)
        => (idx < r.FieldCount) ? (r.GetValue(idx)?.ToString()?.Trim() ?? "") : "";

    int rowCount = 0;
    while (reader.Read())
    {
        rowCount++;

        // Mostrar cabecera para diagnóstico
        if (rowCount == 1) continue; // Ya no necesitamos diagnóstico

        try
        {
            // Nombre (Col B / Index 1)
            string nombre = GetSafeValue(reader, 1);
            if (string.IsNullOrWhiteSpace(nombre)) continue;

            // Precio (Col F / Index 5)
            decimal precio = 0;
            string precioStr = GetSafeValue(reader, 5).Replace("$", "").Replace(",", "").Trim();
            try { precio = Convert.ToDecimal(precioStr); } catch { }
            if (precio <= 0) precio = 1;

            // Stock (No existe columna, asume 0)
            int stock = 0;

            // Id (Col A / Index 0) o generar uno nuevo si está vacío/inválido
            string rawId = GetSafeValue(reader, 0);
            string id;
            if (string.IsNullOrWhiteSpace(rawId) || !Guid.TryParse(rawId, out _))
            {
                id = Guid.NewGuid().ToString();
            }
            else
            {
                id = rawId;
            }

            // Detalles y Categoría
            string detalles = GetSafeValue(reader, 6);
            string categoria = GetSafeValue(reader, 7);

            // Imágenes con EPPlus (Col C=3, D=4, E=5)
            string? img1Url = null;
            string? img2Url = null;
            string? img3Url = null;
            
            // EPPlus From.Row y From.Column son 0-based internamente relativos, sumamos 1
            var rowPics = pictures.Where(p => (p.From.Row + 1) == rowCount).ToList();
            foreach (var pic in rowPics)
            {
                int col = pic.From.Column + 1;
                if (col >= 3 && col <= 5)
                {
                    string ext = "jpg";
                    try { ext = pic.Image.Type.ToString().ToLower(); } catch { }
                    if (ext == "jpeg") ext = "jpg";

                    string fileName = $"{id}_{col}.{ext}";
                    string fullPath = Path.Combine(imagesDir, fileName);

                    File.WriteAllBytes(fullPath, pic.Image.ImageBytes);

                    string relUrl = $"/images/productos/{fileName}";
                    if (col == 3) img1Url = relUrl;
                    if (col == 4) img2Url = relUrl;
                    if (col == 5) img3Url = relUrl;
                }
            }

            const string insertSql = @"
                INSERT INTO ""Producto"" (id, nombre, precio, stock, ""imagen1Url"", ""imagen2Url"", ""imagen3Url"", detalles, categoria)
                VALUES (@Id, @Nombre, @Precio, @Stock, @Img1, @Img2, @Img3, @Detalles, @Categoria)";

            using var cmd = new NpgsqlCommand(insertSql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Nombre", nombre);
            cmd.Parameters.AddWithValue("@Precio", (double)precio);
            cmd.Parameters.AddWithValue("@Stock", stock);
            cmd.Parameters.AddWithValue("@Img1", string.IsNullOrEmpty(img1Url) ? DBNull.Value : img1Url);
            cmd.Parameters.AddWithValue("@Img2", string.IsNullOrEmpty(img2Url) ? DBNull.Value : img2Url);
            cmd.Parameters.AddWithValue("@Img3", string.IsNullOrEmpty(img3Url) ? DBNull.Value : img3Url);
            cmd.Parameters.AddWithValue("@Detalles", string.IsNullOrEmpty(detalles) ? DBNull.Value : detalles);
            cmd.Parameters.AddWithValue("@Categoria", string.IsNullOrEmpty(categoria) ? DBNull.Value : categoria);
            cmd.ExecuteNonQuery();

            importados++;

            if (importados % 100 == 0)
                Console.WriteLine($"  ✓ {importados} productos importados...");
        }
        catch (Exception ex)
        {
            errores++;
            if (errores < 10)
                Console.WriteLine($"  ! Error en fila {rowCount}: {ex.Message}");
        }
    }

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"\n=== ACTUALIZACIÓN COMPLETADA ===");
    Console.WriteLine($"  Registros importados : {importados}");
    Console.WriteLine($"  Errores              : {errores}");
    Console.ResetColor();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\nERROR FATAL: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
    Console.ResetColor();
}
