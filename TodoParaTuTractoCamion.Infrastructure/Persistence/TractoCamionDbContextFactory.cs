using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TodoParaTuTractoCamion.Infrastructure.Persistence
{
    /// <summary>
    /// Factory para que EF Core pueda instanciar el DbContext en tiempo de diseño
    /// (dotnet ef migrations add / dotnet ef database update, etc.)
    /// </summary>
    public class TractoCamionDbContextFactory : IDesignTimeDbContextFactory<TractoCamionDbContext>
    {
        public TractoCamionDbContext CreateDbContext(string[] args)
        {
            // Prioridad: variable de entorno DATABASE_URL → fallback local
            var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
                                   ?? "Host=localhost;Database=TractoCamionDB;Username=postgres;Password=1234;";

            // Soporte para formato postgresql:// (Supabase/Railway)
            if (connectionString.Contains("://"))
            {
                var uri = new Uri(connectionString);
                var userInfo = uri.UserInfo.Split(':');
                var user = userInfo[0];
                var password = userInfo.Length > 1 ? userInfo[1] : "";
                var host = uri.Host;
                var port = uri.Port > 0 ? uri.Port : 5432;
                var database = uri.AbsolutePath.TrimStart('/');
                connectionString = $"Server={host};Port={port};Database={database};User Id={user};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
            }

            var optionsBuilder = new DbContextOptionsBuilder<TractoCamionDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new TractoCamionDbContext(optionsBuilder.Options);
        }
    }
}
