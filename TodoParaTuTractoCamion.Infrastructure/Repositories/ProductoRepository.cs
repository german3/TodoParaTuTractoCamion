using Microsoft.EntityFrameworkCore;
using TodoParaTuTractoCamion.Domain.Entities;
using TodoParaTuTractoCamion.Domain.Interfaces;
using TodoParaTuTractoCamion.Infrastructure.Persistence;

namespace TodoParaTuTractoCamion.Infrastructure.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly TractoCamionDbContext _context;

        public ProductoRepository(TractoCamionDbContext context)
        {
            _context = context;
        }

        public async Task<Producto> GetByIdAsync(Guid id)
        {
            return await _context.Productos.FindAsync(id);
        }

        public async Task<IEnumerable<Producto>> GetAllAsync()
        {
            try
            {
                return await _context.Productos.ToListAsync();
            }
            catch (Exception ex) when (ex.Message.Contains("does not exist"))
            {
                // Auto-healing para entornos de producción desincronizados
                await _context.Database.ExecuteSqlRawAsync("ALTER TABLE \"Producto\" ADD COLUMN IF NOT EXISTS categoria text NULL;");
                await _context.Database.ExecuteSqlRawAsync("ALTER TABLE \"Producto\" ADD COLUMN IF NOT EXISTS detalles text NULL;");
                
                // Intento seguro a tabla original si Producción aún usa la tabla pluralizada
                try { await _context.Database.ExecuteSqlRawAsync("ALTER TABLE \"Productos\" ADD COLUMN IF NOT EXISTS categoria text NULL;"); } catch { }
                try { await _context.Database.ExecuteSqlRawAsync("ALTER TABLE \"Productos\" ADD COLUMN IF NOT EXISTS detalles text NULL;"); } catch { }
                
                return await _context.Productos.ToListAsync();
            }
        }

        public async Task AddAsync(Producto producto)
        {
            await _context.Productos.AddAsync(producto);
        }

        public void Update(Producto producto)
        {
            _context.Productos.Update(producto);
        }

        public void Delete(Producto producto)
        {
            _context.Productos.Remove(producto);
        }
    }
}
