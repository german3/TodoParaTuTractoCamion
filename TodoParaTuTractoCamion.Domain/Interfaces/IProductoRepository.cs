using TodoParaTuTractoCamion.Domain.Entities;

namespace TodoParaTuTractoCamion.Domain.Interfaces
{
    public interface IProductoRepository
    {
        Task<Producto> GetByIdAsync(Guid id);
        Task<IEnumerable<Producto>> GetAllAsync();
        Task AddAsync(Producto producto);
        void Update(Producto producto);
        void Delete(Producto producto);
    }
}
