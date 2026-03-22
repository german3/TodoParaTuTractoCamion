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
            var excelService = services.GetRequiredService<IExcelReaderService>();
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "productos.xlsx");
            var productos = excelService.ReadProductosFromExcel(filePath);
            if (productos.Any())
            {
                context.Productos.AddRange(productos);
                context.SaveChanges();
            }
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred during migration or seeding.");
    }
}

app.Run();
