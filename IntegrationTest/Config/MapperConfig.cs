using AutoMapper;
using WebApi.Infrastructure.Mapper;

namespace IntegrationTest.Config
{
    public static class MapperConfig
    {
        public static IMapper GetMapper() 
        {
            var mockMapper = new MapperConfiguration(cfg => {
                cfg.AddProfile(new AutoMapperProfile());
            });

            var mapper = mockMapper.CreateMapper();
            return mapper;
        }
    }
}
