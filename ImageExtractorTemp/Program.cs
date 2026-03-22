using System;
using System.IO;
using System.Linq;
using ClosedXML.Excel;

class Program
{
    static void Main()
    {
        string xlsxPath = @"D:\Codrava\Proyecto\Lista Productos.xlsx";
        if (!File.Exists(xlsxPath))
        {
            Console.WriteLine("File not found");
            return;
        }

        using var workbook = new XLWorkbook(xlsxPath);
        var ws = workbook.Worksheet(1);
        
        var pics = ws.Pictures.Where(p => p.TopLeftCell.Address.RowNumber == 329).ToList();
        Console.WriteLine($"Pictures found in row 329: {pics.Count}");
        
        if (pics.Any())
        {
            var pic = pics.First();
            Console.WriteLine($"Picture name: {pic.Name}");
            // Let's see if ImageStream exists:
            using var ms = new MemoryStream();
            pic.ImageStream.CopyTo(ms);
            Console.WriteLine($"Image length: {ms.Length} bytes");
        }
    }
}
