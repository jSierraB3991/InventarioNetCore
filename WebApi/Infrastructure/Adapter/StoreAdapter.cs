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
    public class StoreAdapter : GenericAdapter<Store>, IStoreService
    {
        public StoreAdapter(AppDbContext context) : base(context)
        {
        }

        public override string Name => "Store";

        public override async Task<Store> FindByIdAllData(Guid id)
        {
            await GetByIdAsync(id);
            var entity = await DbEntity
                            .Include(store => store.ProductStores).ThenInclude(ps => ps.Product)
                            .Where(store => store.Id.Equals(id)).FirstOrDefaultAsync();
            return entity;
        }

        public override async Task VerifyData(Store store)
        {
            var entityName = await DbEntity.AsNoTracking().FirstOrDefaultAsync(p => p.Name.Equals(store.Name));
            if (entityName != null && !entityName.Id.Equals(store.Id))
            {
                throw new DuplicateDataException($"This Store {entityName.Name} already in inventary with en capacity {entityName.MaxCapacity}");
            }
        }
    }
}
