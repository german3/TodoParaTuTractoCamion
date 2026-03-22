using MediatR;
using AutoMapper;
using TodoParaTuTractoCamion.Application.DTOs;
using TodoParaTuTractoCamion.Domain.Interfaces;

namespace TodoParaTuTractoCamion.Application.Products.Queries.GetProductos
{
    public record GetProductosQuery() : IRequest<IEnumerable<ProductoDto>>;

    public class GetProductosHandler : IRequestHandler<GetProductosQuery, IEnumerable<ProductoDto>>
    {
        private readonly IProductoRepository _repository;
        private readonly IMapper _mapper;

        public GetProductosHandler(IProductoRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductoDto>> Handle(GetProductosQuery request, CancellationToken cancellationToken)
        {
            var productos = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductoDto>>(productos);
        }
    }
}
