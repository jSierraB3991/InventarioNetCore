namespace WebApi.Domain.Model
{
    public class AuditableEntity
    {
        public System.Guid Id { get; set; }

        public System.DateTime CreateAt { get; set; }

        public System.DateTime ModifyDate { get; set; }
    }
}
