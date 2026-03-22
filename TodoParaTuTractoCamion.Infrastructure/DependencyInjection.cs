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
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Si la cadena viene en formato postgres:// (típico de Railway/Heroku)
            if (connectionString != null && connectionString.StartsWith("postgres://"))
            {
                var uri = new Uri(connectionString);
                var userInfo = uri.UserInfo.Split(':');
                connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true;";
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
