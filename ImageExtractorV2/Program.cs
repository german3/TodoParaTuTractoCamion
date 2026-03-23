using System;
using System.IO;
using System.Linq;
using ClosedXML.Excel;

class Program
{
    static void Main()
    {
        string xlsxPath = @"D:\Codrava\Proyecto\Lista Productos.xlsx";
        string outputDir = @"D:\Codrava\Proyecto\TodoParaTuTractoCamion.BlazorWasm\wwwroot\images\productos";
        
        using var workbook = new XLWorkbook(xlsxPath);
        var ws = workbook.Worksheet(1);
        
        int[] targetRows = { 1, 44, 91, 146, 277, 284, 385 };
        
        Console.WriteLine("Extracting images from specific rows...");

        foreach (int rowNum in targetRows)
        {
            var row = ws.Row(rowNum);
            string colA = row.Cell(1).GetValue<string>();
            string colB = row.Cell(2).GetValue<string>();
            
            Console.WriteLine($"Row {rowNum}: A='{colA}', B='{colB}'");
            
            // Find pictures where TopLeft is in this row
            var pictures = ws.Pictures.Where(p => p.TopLeftCell.Address.RowNumber == rowNum).ToList();
            
            if (!pictures.Any())
            {
                pictures = ws.Pictures.Where(p => Math.Abs(p.TopLeftCell.Address.RowNumber - rowNum) <= 1).ToList();
            }

            int idx = 1;
            foreach (var pic in pictures)
            {
                string extension = pic.Format.ToString().ToLower().Contains("png") ? "png" : "jpg";
                string fileName = $"img_fixed_r{rowNum}_{idx}.{extension}";
                string fullPath = Path.Combine(outputDir, fileName);
                using var ms = new MemoryStream();
                pic.ImageStream.CopyTo(ms);
                File.WriteAllBytes(fullPath, ms.ToArray());
                Console.WriteLine($"   -> Saved {fileName}");
                idx++;
            }
        }
    }
}
