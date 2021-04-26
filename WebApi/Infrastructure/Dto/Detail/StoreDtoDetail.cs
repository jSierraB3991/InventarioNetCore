using System.Collections.Generic;

namespace WebApi.Infrastructure.Dto.Detail
{
    public class StoreDtoDetail: StoreDto
    {
        public ICollection<ProductDto> Products { get; set; }

        public int Van { get; set; }

        public int Faltan { get; set; }
    }
}
