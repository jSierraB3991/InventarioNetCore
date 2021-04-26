namespace WebApi.Infrastructure.Dto
{
    public class BaseDto
    {
        public System.Guid Id { get; set; }

        public System.DateTime CreateAt { get; set; }

        public System.DateTime ModifyDate { get; set; }
    }
}
