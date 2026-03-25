using Microsoft.EntityFrameworkCore;
using TodoParaTuTractoCamion.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

// Setup configuration to get connection string
var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var connectionString = "Host=localhost;Database=TodoParaTuTractoCamion;Username=postgres;Password=postgres"; // Standard fallback for this project

var optionsBuilder = new DbContextOptionsBuilder<TractoCamionDbContext>();
optionsBuilder.UseNpgsql(connectionString);

using (var context = new TractoCamionDbContext(optionsBuilder.Options))
{
    var products = context.Productos.ToList();
    var random = new Random();
    
    foreach (var product in products)
    {
        // Set stock between 10 and 39
        var newStock = random.Next(10, 40);
        
        // Use reflection or just direct set if it's public. 
        // In this project, Stock is a value object but likely has a public constructor or similar.
        // Let's check the entity again.
    }
    
    // Actually, SQL is easier if I can run it via dotnet ef or similar.
    // Or I can just execute raw SQL via the context.
    context.Database.ExecuteSqlRaw("UPDATE \"Productos\" SET \"Stock_Value\" = floor(random() * 30 + 10)");
    
    Console.WriteLine("Stock updated successfully.");
}
