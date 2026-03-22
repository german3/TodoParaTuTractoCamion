using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoParaTuTractoCamion.Domain.Interfaces;
using TodoParaTuTractoCamion.Infrastructure.Persistence;
using TodoParaTuTractoCamion.Infrastructure.Repositories;
using TodoParaTuTractoCamion.Infrastructure.Services;

namespace TodoParaTuTractoCamion.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Obtener de DefaultConnection o DATABASE_URL
            var connectionString = configuration.GetConnectionString("DefaultConnection") 
                                   ?? configuration["DATABASE_URL"];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = "Host=localhost;Database=TractoCamionDB;Username=postgres;Password=postgres;";
            }

            // Limpiar posibles espacios o comillas accidentales
            connectionString = connectionString.Trim().Trim('"').Trim('\'');

            Console.WriteLine($"[DEBUG] DB Connection starts with: {(connectionString.Length > 10 ? connectionString.Substring(0, 10) : connectionString)}...");

            // Soporte para formato postgres:// (Railway)
            if (connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
            {
                try 
                {
                    var uri = new Uri(connectionString);
                    var userInfo = uri.UserInfo.Split(':');
                    var user = userInfo[0];
                    var password = userInfo.Length > 1 ? userInfo[1] : "";
                    
                    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={user};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to parse postgres:// URL: {ex.Message}");
                }
            }

            services.AddDbContext<TractoCamionDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<IProductoRepository, ProductoRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IExcelReaderService, ExcelReaderService>();

            return services;
        }
    }
}
