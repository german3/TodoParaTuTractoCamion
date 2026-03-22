using MediatR;
using TodoParaTuTractoCamion.Application.Common;
using TodoParaTuTractoCamion.Domain.Interfaces;

namespace TodoParaTuTractoCamion.Application.Products.Commands.ReduceStock
{
    public record ReduceStockCommand(Guid Id, int Cantidad) : IRequest<Result<bool>>;

    public class ReduceStockHandler : IRequestHandler<ReduceStockCommand, Result<bool>>
    {
        private readonly IProductoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ReduceStockHandler(IProductoRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(ReduceStockCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var producto = await _repository.GetByIdAsync(request.Id);
                if (producto == null) return Result<bool>.Failure("Producto no encontrado.");

                producto.ReducirStock(request.Cantidad);
                _repository.Update(producto);
                await _unitOfWork.SaveChangesAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(ex.Message);
            }
        }
    }
}
