using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain.Model
{
    public class Store : AuditableEntity
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Direction { get; set; }

        [Required]
        public int MaxCapacity { get; set; }

        public ICollection<ProductStore> ProductStores { get; set; }
    }
}
