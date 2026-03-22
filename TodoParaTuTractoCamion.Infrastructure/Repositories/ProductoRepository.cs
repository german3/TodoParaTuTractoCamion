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
            return await _context.Productos.ToListAsync();
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
