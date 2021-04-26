namespace WebApi.Infrastructure.Dto
{
    public class MovementDto: BaseDto
    {
        public System.DateTime MovementDate { get; set; }

        public Domain.enums.MovementType Type { get; set; }

        public ProductDto Product { get; set; }

        public StoreDto Store { get; set; }

        public int Stock { get; set; }
    }
}
