using AutoMapper;
using WebApi.Domain.Service;
using WebApi.Infrastructure.Adapter;
using WebApi.Infrastructure.Controllers;
using WebApi.Infrastructure.Repository;

namespace IntegrationTest.Config
{
    public class MovementControllerConfig
    {
        public static MovementController GetMovementController(AppDbContext dbContext, IProductService productService, IStoreService storeService, IMapper mapper)
        {
            IProductStoreService productStoreService = new ProductStoreAdapter(dbContext);
            IMovementService movementService = new MovementAdapter(dbContext, productService, storeService, productStoreService);
            return new MovementController(movementService, mapper, Serilog.Log.Logger);
        }
    }
}
