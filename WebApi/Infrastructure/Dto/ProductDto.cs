namespace WebApi.Infrastructure.Dto
{
    public class ProductDto: BaseDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Sku { get; set; }

        public long Stock { get; set; }
    }
}
