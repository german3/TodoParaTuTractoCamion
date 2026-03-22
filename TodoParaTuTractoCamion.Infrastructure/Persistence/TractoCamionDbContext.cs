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
                entity.Property(e => e.Id).HasColumnName("id");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Imagen1Url).HasColumnName("imagen1url");
                entity.Property(e => e.Imagen2Url).HasColumnName("imagen2url");
                entity.Property(e => e.Imagen3Url).HasColumnName("imagen3url");

                entity.OwnsOne(p => p.Precio, p =>
                {
                    p.Property(x => x.Value).HasColumnName("precio").HasPrecision(18, 2);
                });

                entity.OwnsOne(p => p.Stock, p =>
                {
                    p.Property(x => x.Value).HasColumnName("stock");
                });
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
