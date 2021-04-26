using System.Collections.Generic;

namespace WebApi.Infrastructure.Dto.Detail
{
    public class ProductDtoDetail: ProductDto
    {
        public ICollection<StoreDto> Stores { get; set; }
    }
}
