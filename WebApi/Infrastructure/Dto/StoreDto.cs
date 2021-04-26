namespace WebApi.Infrastructure.Dto
{
    public class StoreDto: BaseDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Direction { get; set; }

        public int MaxCapacity { get; set; }

        public int Stock { get; set; }
    }
}
