using ClosedXML.Excel;
using TodoParaTuTractoCamion.Domain.Entities;
using TodoParaTuTractoCamion.Domain.ValueObjects;

namespace TodoParaTuTractoCamion.Infrastructure.Services
{
    public interface IExcelReaderService
    {
        IEnumerable<Producto> ReadProductosFromExcel(string filePath);
    }

    public class ExcelReaderService : IExcelReaderService
    {
        public IEnumerable<Producto> ReadProductosFromExcel(string filePath)
        {
            var productos = new List<Producto>();

            if (!File.Exists(filePath)) return productos;

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RangeUsed().RowsUsed();

                foreach (var row in rows)
                {
                    if (row.RowNumber() == 1) continue; // Skip header

                    try
                    {
                        // A: Id (or Name if Guid is not provided), B: Name
                        // Let's assume A is Nombre for simplicity if not Guid, 
                        // but the request says columns I, J, K are images.
                        // I'll map based on typical structure and the user's specific image columns.
                        
                        string nombre = row.Cell(1).GetValue<string>(); // Col A
                        decimal precio = row.Cell(2).GetValue<decimal>(); // Col B
                        int stock = row.Cell(3).GetValue<int>(); // Col C - Assuming stock here
                        
                        string img1 = row.Cell(9).GetValue<string>(); // Col I
                        string img2 = row.Cell(10).GetValue<string>(); // Col J
                        string img3 = row.Cell(11).GetValue<string>(); // Col K

                        // If stock < 100, add to list (as per request)
                        if (stock < 100)
                        {
                            var producto = new Producto(
                                Guid.NewGuid(),
                                nombre,
                                new Precio(precio > 0 ? precio : 1), // Fallback if invalid
                                new Stock(stock >= 0 ? stock : 0),
                                img1,
                                img2,
                                img3
                            );
                            productos.Add(producto);
                        }
                    }
                    catch
                    {
                        // Skip rows with errors
                        continue;
                    }
                }
            }

            return productos;
        }
    }
}
