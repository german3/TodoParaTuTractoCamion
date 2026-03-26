using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Npgsql;

namespace Exporter
{
    public record ProductoJsonDto(
        Guid Id, string Nombre, decimal Precio, int Stock, 
        string Imagen1Url, string Imagen2Url, string Imagen3Url);

    class Program
    {
        static void Main(string[] args)
        {
            var ds = new List<ProductoJsonDto>();
            string connString = "Host=localhost;Database=TractoCamionDB;Username=postgres;Password=1234;";

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(@"SELECT id, nombre, precio, stock, ""imagen1Url"", ""imagen2Url"", ""imagen3Url"" FROM ""Producto""", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var idStr = reader.GetString(0);
                        var id = Guid.Parse(idStr);
                        var nombre = reader.GetString(1);
                        var precio = Convert.ToDecimal(reader.GetDouble(2));
                        var stock = reader.GetInt32(3);
                        var img1 = reader.IsDBNull(4) ? null : reader.GetString(4);
                        var img2 = reader.IsDBNull(5) ? null : reader.GetString(5);
                        var img3 = reader.IsDBNull(6) ? null : reader.GetString(6);

                        ds.Add(new ProductoJsonDto(id, nombre, precio, stock, img1, img2, img3));
                    }
                }
            }

            var json = JsonSerializer.Serialize(ds, new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            
            var targetPath = @"D:\Codrava\Proyecto\productos_backup.json";
            File.WriteAllText(targetPath, json);
            
            Console.WriteLine($"Exported {ds.Count} products to {targetPath}");
        }
    }
}
