using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Domain.Exception;
using WebApi.Domain.Model;
using WebApi.Domain.Service;
using WebApi.Infrastructure.Repository;

namespace WebApi.Infrastructure.Adapter
{
    public class ProductAdapter : GenericAdapter<Product>, IProductService
    {
        public ProductAdapter(AppDbContext context) : base(context)
        {
        }

        public override string Name => "Product";

        public override async Task<Product> FindByIdAllData(Guid id)
        {
            await GetByIdAsync(id);
            var entity = await DbEntity
                            .Include(p => p.ProductStores).ThenInclude(ps => ps.Store)
                            .Where(product => product.Id.Equals(id)).FirstOrDefaultAsync();
            return entity;
        }

        public override async Task VerifyData(Product entity)
        {
            var entityName = await DbEntity.AsNoTracking().FirstOrDefaultAsync(p => p.Name.Equals(entity.Name));
            if (entityName != null && !entityName.Id.Equals(entity.Id))
            {
                throw new DuplicateDataException($"This Product {entityName.Name} already in inventary with Id {entityName.Id}");
            }
        }
    }
}
