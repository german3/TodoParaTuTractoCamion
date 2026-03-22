using MediatR;
using AutoMapper;
using TodoParaTuTractoCamion.Application.DTOs;
using TodoParaTuTractoCamion.Domain.Interfaces;

namespace TodoParaTuTractoCamion.Application.Products.Queries.GetProductoById
{
    public record GetProductoByIdQuery(Guid Id) : IRequest<ProductoDto>;

    public class GetProductoByIdHandler : IRequestHandler<GetProductoByIdQuery, ProductoDto>
    {
        private readonly IProductoRepository _repository;
        private readonly IMapper _mapper;

        public GetProductoByIdHandler(IProductoRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ProductoDto> Handle(GetProductoByIdQuery request, CancellationToken cancellationToken)
        {
            var producto = await _repository.GetByIdAsync(request.Id);
            return _mapper.Map<ProductoDto>(producto);
        }
    }
}
