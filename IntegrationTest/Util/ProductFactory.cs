using WebApi.Domain.Model;
using WebApi.Infrastructure.Dto;

namespace IntegrationTest.Util
{
    internal static class ProductFactory
    {
        public static Product FakerProduct()
        {
            return new Product
            {
                Name = Faker.Name.FullName(),
                Sku = Faker.Company.Name(),
                Description = Faker.Lorem.GetFirstWord()
            };
        }

        public static ProductDto FluentDtoBad()
        {
            return new ProductDto
            {
                Description = null,
                Name = "a",
                Sku = string.Empty
            };
        }
    }
}
