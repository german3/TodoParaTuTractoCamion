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

            // Soporte para formato postgres:// o postgresql:// (Supabase/Railway)
            if (connectionString.Contains("://"))
            {
                try 
                {
                    var uri = new Uri(connectionString);
                    var userInfo = uri.UserInfo.Split(':');
                    var user = userInfo[0];
                    var password = userInfo.Length > 1 ? userInfo[1] : "";
                    var host = uri.Host;
                    var port = uri.Port;
                    var database = uri.AbsolutePath.TrimStart('/');

                    // Conexión formateada para Npgsql con SSL para Supabase
                    connectionString = $"Host={host};Port={port};Database={database};Username={user};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
                    
                    Console.WriteLine($"[DEBUG] Parsed Connection: Host={host}, Port={port}, DB={database}, User={user}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Failed to parse DATABASE_URL: {ex.Message}");
                }
            }
            else 
            {
                Console.WriteLine("[DEBUG] Using direct connection string (no URI format detected).");
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
