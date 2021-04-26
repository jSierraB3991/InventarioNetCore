namespace WebApi.Domain.Model
{
    public class Movement : AuditableEntity
    {
        public System.DateTime MovementDate { get; set; }

        public enums.MovementType Type { get; set; }

        public Product Product { get; set; }

        public int Stock { get; set; }

        public Store Store { get; set; }

        public string TranslateId { get; set; }
    }
}
