using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain.Model
{
    public class Product : AuditableEntity
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public string Sku { get; set; }

        public ICollection<ProductStore> ProductStores { get; set; }
    }
}
