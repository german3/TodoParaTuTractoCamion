using MediatR;
using TodoParaTuTractoCamion.Application.Common;
using TodoParaTuTractoCamion.Domain.Entities;
using TodoParaTuTractoCamion.Domain.Interfaces;
using TodoParaTuTractoCamion.Domain.ValueObjects;

namespace TodoParaTuTractoCamion.Application.Products.Commands.CreateProducto
{
    public record CreateProductoCommand(string Nombre, decimal Precio, int Stock, string Imagen1Url, string Imagen2Url, string Imagen3Url) : IRequest<Result<Guid>>;

    public class CreateProductoHandler : IRequestHandler<CreateProductoCommand, Result<Guid>>
    {
        private readonly IProductoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductoHandler(IProductoRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateProductoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var producto = new Producto(
                    Guid.NewGuid(),
                    request.Nombre,
                    new Precio(request.Precio),
                    new Stock(request.Stock),
                    request.Imagen1Url,
                    request.Imagen2Url,
                    request.Imagen3Url
                );

                await _repository.AddAsync(producto);
                await _unitOfWork.SaveChangesAsync();

                return Result<Guid>.Success(producto.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }
        }
    }
}
