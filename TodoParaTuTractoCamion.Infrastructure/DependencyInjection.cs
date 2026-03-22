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
            // Intentar obtener de DefaultConnection o directamente de DATABASE_URL (común en Railway)
            var connectionString = configuration.GetConnectionString("DefaultConnection") 
                                   ?? configuration["DATABASE_URL"];

            if (string.IsNullOrEmpty(connectionString))
            {
                // Fallback final para evitar errores de nulo si no hay nada configurado
                connectionString = "Host=localhost;Database=TractoCamionDB;Username=postgres;Password=postgres;";
            }

            // Si la cadena viene en formato postgres:// (típico de Railway/Heroku)
            if (connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
            {
                var uri = new Uri(connectionString);
                var userInfo = uri.UserInfo.Split(':');
                var user = userInfo[0];
                var password = userInfo.Length > 1 ? userInfo[1] : "";
                
                connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={user};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
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
