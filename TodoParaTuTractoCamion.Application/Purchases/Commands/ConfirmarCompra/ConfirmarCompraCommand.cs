using MediatR;
using TodoParaTuTractoCamion.Application.Common;
using TodoParaTuTractoCamion.Application.DTOs;
using TodoParaTuTractoCamion.Domain.Interfaces;

namespace TodoParaTuTractoCamion.Application.Purchases.Commands.ConfirmarCompra
{
    public record ConfirmarCompraCommand(List<CompraDto> Items) : IRequest<Result<bool>>;

    public class ConfirmarCompraHandler : IRequestHandler<ConfirmarCompraCommand, Result<bool>>
    {
        private readonly IProductoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmarCompraHandler(IProductoRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(ConfirmarCompraCommand request, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var item in request.Items)
                {
                    var producto = await _repository.GetByIdAsync(item.ProductoId);
                    if (producto == null) return Result<bool>.Failure($"Producto {item.ProductoId} no encontrado.");

                    producto.ReducirStock(item.Cantidad);
                    _repository.Update(producto);
                }

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
