using WebApi.Domain.Model;
using WebApi.Domain.Service;
using WebApi.Infrastructure.Repository;

namespace WebApi.Infrastructure.Adapter
{
    public class ProductStoreAdapter : GenericAdapter<ProductStore>, IProductStoreService
    {
        public ProductStoreAdapter(AppDbContext context) : base(context)
        {

        }
    }
}
