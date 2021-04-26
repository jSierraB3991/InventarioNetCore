namespace WebApi.Domain.Model
{
    public class ProductStore : AuditableEntity
    {
        public Product Product { get; set; }

        public Store Store { get; set; }

        public long Stock { get; set; }
    }
}
