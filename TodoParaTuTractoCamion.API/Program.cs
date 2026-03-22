using Serilog;
using TodoParaTuTractoCamion.Application;
using TodoParaTuTractoCamion.Infrastructure;
using TodoParaTuTractoCamion.Infrastructure.Persistence;
using TodoParaTuTractoCamion.Infrastructure.Services;
using TodoParaTuTractoCamion.API.Middleware;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
app.MapControllers();

// Seed Logic
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<TractoCamionDbContext>();
        context.Database.Migrate();

        if (!context.Productos.Any())
        {
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "productos_backup.json");
            if (File.Exists(jsonPath))
            {
                var jsonOptions = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var jsonText = File.ReadAllText(jsonPath);
                
                // Usar un DTO para deserializar porque el JSON es plano pero el Dominio usa Value Objects
                var dtos = System.Text.Json.JsonSerializer.Deserialize<List<ProductoJsonDto>>(jsonText, jsonOptions);
                
                if (dtos != null && dtos.Any())
                {
                    var productos = dtos.Select(d => new TodoParaTuTractoCamion.Domain.Entities.Producto(
                        d.Id,
                        d.Nombre,
                        new TodoParaTuTractoCamion.Domain.ValueObjects.Precio(d.Precio),
                        new TodoParaTuTractoCamion.Domain.ValueObjects.Stock(d.Stock),
                        d.Imagen1Url,
                        d.Imagen2Url,
                        d.Imagen3Url
                    )).ToList();

                    context.Productos.AddRange(productos);
                    context.SaveChanges();
                    Log.Information($"Seeded {productos.Count} products from JSON.");
                }
            }
            else
            {
                var excelService = services.GetRequiredService<IExcelReaderService>();
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "productos.xlsx");
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
            }
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred during migration or seeding.");
    }
}

app.Run();

// DTO para carga de datos inicial
public record ProductoJsonDto(
    Guid Id,
    string Nombre,
    decimal Precio,
    int Stock,
    string Imagen1Url,
    string Imagen2Url,
    string Imagen3Url
);
