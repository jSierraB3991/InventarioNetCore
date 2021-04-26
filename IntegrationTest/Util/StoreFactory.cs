using WebApi.Domain.Model;
using WebApi.Infrastructure.Dto;

namespace IntegrationTest.Util
{
    internal static class StoreFactory
    {
        public static Store FakerStore()
        {
            return new Store
            {
                Description = Faker.Name.FullName(),
                Name = Faker.Name.First(),
                Direction = Faker.Address.StreetAddress(),
                MaxCapacity = Faker.RandomNumber.Next(10, 1000)
            };
        }

        public static StoreDto FluentDtoBad()
        {
            return new StoreDto
            {
                Direction = null,
                Description = null,
                Name = "abc",
                MaxCapacity = 0
            };
        }
    }
}
