using WebApi.Domain.Model;
using WebApi.Infrastructure.Dto;

namespace WebApi.Infrastructure.Mapper
{
    public static class ProductMapper
    {
        public static ProductDto GetProductDtoByStore(ProductStore productStore)
        {
            return new ProductDto
            {
                Id = productStore.Product.Id,
                CreateAt = productStore.Product.CreateAt.ToLocalTime(),
                Description = productStore.Product.Description,
                ModifyDate = productStore.Product.ModifyDate.ToLocalTime(),
                Name = productStore.Product.Name,
                Sku = productStore.Product.Sku,
                Stock = productStore.Stock
            };
        }
    }
}
