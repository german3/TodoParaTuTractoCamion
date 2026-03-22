using Microsoft.EntityFrameworkCore;
using TodoParaTuTractoCamion.Domain.Entities;

namespace TodoParaTuTractoCamion.Infrastructure.Persistence
{
    public class TractoCamionDbContext : DbContext
    {
        public TractoCamionDbContext(DbContextOptions<TractoCamionDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("Producto");
                entity.HasKey(e => e.Id);

                entity.OwnsOne(p => p.Precio, p =>
                {
                    p.Property(x => x.Value).HasColumnName("Precio").HasPrecision(18, 2);
                });

                entity.OwnsOne(p => p.Stock, p =>
                {
                    p.Property(x => x.Value).HasColumnName("Stock");
                });
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
