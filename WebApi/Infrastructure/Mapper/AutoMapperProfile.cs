using System.Linq;
using WebApi.Domain.Model;
using WebApi.Infrastructure.Dto;
using WebApi.Infrastructure.Dto.Detail;

namespace WebApi.Infrastructure.Mapper
{
    public class AutoMapperProfile : AutoMapper.Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dto => dto.CreateAt, opt => opt.MapFrom(entity => entity.CreateAt.ToLocalTime()))
                .ForMember(dto => dto.ModifyDate, opt => opt.MapFrom(entity => entity.ModifyDate.ToLocalTime()));
            CreateMap<ProductDto, Product>();


            CreateMap<Store, StoreDto>()
                .ForMember(dto => dto.CreateAt, opt => opt.MapFrom(entity => entity.CreateAt.ToLocalTime()))
                .ForMember(dto => dto.ModifyDate, opt => opt.MapFrom(entity => entity.ModifyDate.ToLocalTime()))
                .ForMember(dto => dto.Stock, opt => opt.MapFrom(entity => entity.ProductStores.Sum(ps => ps.Stock)));
            CreateMap<StoreDto, Store>();


            CreateMap<Movement, MovementDto>()
                .ForMember(dto => dto.CreateAt, opt => opt.MapFrom(entity => entity.CreateAt.ToLocalTime()))
                .ForMember(dto => dto.ModifyDate, opt => opt.MapFrom(entity => entity.ModifyDate.ToLocalTime()));
            CreateMap<MovementDto, Movement>();

            CreateMap<Store, StoreDtoDetail>()
                .ForMember(dto => dto.CreateAt, opt => opt.MapFrom(entity => entity.CreateAt.ToLocalTime()))
                .ForMember(dto => dto.ModifyDate, opt => opt.MapFrom(entity => entity.ModifyDate.ToLocalTime()))
                .ForMember(dto => dto.Van, opt => opt.MapFrom(entity => entity.ProductStores.Sum(ps => ps.Stock)))
                .ForMember(dto => dto.Faltan, opt => opt.MapFrom(entity => entity.MaxCapacity - entity.ProductStores.Sum(ps => ps.Stock)))
                .ForMember(dto => dto.Products, opt => opt.MapFrom(entity => entity.ProductStores.Select(ProductMapper.GetProductDtoByStore).ToList()));
            CreateMap<StoreDto, Store>();


            CreateMap<Product, ProductDtoDetail>()
                .ForMember(dto => dto.CreateAt, opt => opt.MapFrom(entity => entity.CreateAt.ToLocalTime()))
                .ForMember(dto => dto.ModifyDate, opt => opt.MapFrom(entity => entity.ModifyDate.ToLocalTime()))
                .ForMember(dto => dto.Stores, opt => opt.MapFrom(entity => entity.ProductStores.Select(ps => ps.Store).ToList()))
                .ForMember(dto => dto.Stock, opt => opt.MapFrom(entity => entity.ProductStores.Sum(ps => ps.Stock)));
        }
    }
}
