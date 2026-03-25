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
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasConversion(
                        v => v.ToString(),
                        v => Guid.Parse(v));
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Imagen1Url).HasColumnName("imagen1Url");
                entity.Property(e => e.Imagen2Url).HasColumnName("imagen2Url");
                entity.Property(e => e.Imagen3Url).HasColumnName("imagen3Url");
                entity.Property(e => e.Detalles).HasColumnName("detalles");
                entity.Property(e => e.Categoria).HasColumnName("categoria");

                entity.OwnsOne(p => p.Precio, p =>
                {
                    p.Property(x => x.Value)
                     .HasColumnName("precio")
                     .HasPrecision(18, 2)
                     .HasConversion<double>();
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
