using Serilog;
using TodoParaTuTractoCamion.Application;
using TodoParaTuTractoCamion.Infrastructure;
using TodoParaTuTractoCamion.Infrastructure.Persistence;
using TodoParaTuTractoCamion.Infrastructure.Services;
using TodoParaTuTractoCamion.API.Middleware;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.IO;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

// Seed Logic
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<TractoCamionDbContext>();
        context.Database.Migrate();

        try
        {
            // Fallback robusto para asegurar que las columnas existan en Producción
            // en caso de que el historial de migraciones de EF esté desincronizado.
            context.Database.ExecuteSqlRaw("ALTER TABLE \"Producto\" ADD COLUMN IF NOT EXISTS categoria text NULL;");
            context.Database.ExecuteSqlRaw("ALTER TABLE \"Producto\" ADD COLUMN IF NOT EXISTS detalles text NULL;");
            Log.Information("Raw SQL column verification completed.");
        }
        catch (Exception sqlEx)
        {
            Log.Warning(sqlEx, "Raw SQL column fallback failed, columns may already exist.");
        }

        if (!context.Productos.Any())
        {
            Log.Information("No products found in database. Starting seeding process...");
            
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var jsonPath = Path.Combine(baseDir, "productos_backup.json");
            
            Log.Information($"Searching for seed file at: {jsonPath}");

            if (File.Exists(jsonPath))
            {
                Log.Information("Seed file found. Deserializing...");
                var jsonOptions = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var jsonText = File.ReadAllText(jsonPath);
                
                var dtos = System.Text.Json.JsonSerializer.Deserialize<List<ProductoJsonDto>>(jsonText, jsonOptions);
                
                if (dtos != null && dtos.Any())
                {
                    Log.Information($"Mapped {dtos.Count} products from JSON. Saving to database...");
                    var productos = dtos.Select(d => new TodoParaTuTractoCamion.Domain.Entities.Producto(
                        d.Id,
                        d.Nombre,
                        new TodoParaTuTractoCamion.Domain.ValueObjects.Precio(d.Precio),
                        new TodoParaTuTractoCamion.Domain.ValueObjects.Stock(d.Stock),
                        d.Imagen1Url,
                        d.Imagen2Url,
                        d.Imagen3Url,
                        d.Detalles,
                        d.Categoria
                    )).ToList();

                    context.Productos.AddRange(productos);
                    context.SaveChanges();
                    Log.Information($"Successfully seeded {productos.Count} products from JSON.");
                }
                else
                {
                    Log.Warning("Seed file was found but contained no products or failed to deserialize.");
                }
            }
            else
            {
                Log.Warning($"Seed file not found at {jsonPath}. Checking backup Excel path...");
                var excelService = services.GetRequiredService<IExcelReaderService>();
                var filePath = Path.Combine(baseDir, "..", "productos.xlsx");
                if (File.Exists(filePath))
                {
                    var productos = excelService.ReadProductosFromExcel(filePath);
                    if (productos.Any())
                    {
                        context.Productos.AddRange(productos);
                        context.SaveChanges();
                        Log.Information($"Seeded {productos.Count()} products from Excel.");
                    }
                }
                else 
                {
                    Log.Warning($"Excel backup not found at {filePath}. Seeding skipped.");
                }
            }
        }
        else 
        {
            Log.Information("Database already contains products. Seeding skipped.");
        }
    }
    catch (Exception ex)
    {
        var message = ex.Message;
        if (ex.InnerException != null) message += " | Inner: " + ex.InnerException.Message;
        Log.Error(ex, $"An error occurred during migration or seeding: {message}");
    }
}

app.Run();

// DTO para carga de datos inicial
public record ProductoJsonDto(
    Guid Id,
    string Nombre,
    decimal Precio,
    int Stock,
    string? Imagen1Url,
    string? Imagen2Url,
    string? Imagen3Url,
    string? Categoria = null,
    string? Detalles = null
);
