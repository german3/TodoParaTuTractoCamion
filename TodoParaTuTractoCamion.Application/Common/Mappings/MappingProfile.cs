using AutoMapper;
using TodoParaTuTractoCamion.Application.DTOs;
using TodoParaTuTractoCamion.Domain.Entities;

namespace TodoParaTuTractoCamion.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Producto, ProductoDto>()
                .ForMember(dest => dest.Precio, opt => opt.MapFrom(src => src.Precio.Value))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Stock.Value));
        }
    }
}
