using AutoMapper;
using IntegrationTest.Config;
using IntegrationTest.Util;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Domain.Service;
using WebApi.Infrastructure.Adapter;
using WebApi.Infrastructure.Controllers;
using WebApi.Infrastructure.Dto;
using WebApi.Infrastructure.Dto.Detail;
using WebApi.Infrastructure.Dto.Request;
using Xunit;

namespace IntegrationTest.Controller
{
    public class MovementControllerTestBuyProducts
    {
        private readonly IMapper mapper;
        private readonly IProductService productService;
        private readonly IStoreService storeService;
        private readonly MovementController controller;

        public MovementControllerTestBuyProducts()
        {
            mapper = MapperConfig.GetMapper();
            WebApi.Infrastructure.Repository.AppDbContext dbContext = TestDb.GetContext();
            productService = new ProductAdapter(dbContext);
            storeService = new StoreAdapter(dbContext);
            IProductStoreService productStoreService = new ProductStoreAdapter(dbContext);
            IMovementService movementService = new MovementAdapter(dbContext, productService, storeService, productStoreService);
            controller = new MovementController(movementService, mapper, Serilog.Log.Logger);
        }

        [Fact]
        private async Task BuyProductInStoreOk()
        {
            var store = await storeService.SaveAsync(StoreFactory.FakerStore());
            var product = await productService.SaveAsync(ProductFactory.FakerProduct());
            var buyCuanty = store.MaxCapacity / 2;

            var saleBuy = new SaleBuy { Product = product.Id, Store = store.Id, Stock = buyCuanty };

            var response = await controller.BuyProducts(saleBuy);

            var result = response as OkObjectResult;
            Assert.NotNull(result);
            var objectResponse = result.Value as ProductDtoDetail;
            Assert.Equal(200, result.StatusCode.Value);
            Assert.Equal(product.Description, objectResponse.Description);
            Assert.Equal(product.Id, objectResponse.Id);
            Assert.Equal(product.Name, objectResponse.Name);
            Assert.Equal(product.Sku, objectResponse.Sku);

            Assert.Equal(buyCuanty, objectResponse.Stock);
            Assert.Equal(store.Name, objectResponse.Stores.FirstOrDefault().Name);
            Assert.Equal(store.Id, objectResponse.Stores.FirstOrDefault().Id);
            Assert.Equal(store.Description, objectResponse.Stores.FirstOrDefault().Description);
            Assert.Equal(store.Direction, objectResponse.Stores.FirstOrDefault().Direction);
            Assert.Equal(store.MaxCapacity, objectResponse.Stores.FirstOrDefault().MaxCapacity);
            Assert.Equal(buyCuanty, objectResponse.Stores.FirstOrDefault().Stock);
        }


        [Fact]
        private async Task BuyProductInStoreBadUpCapacityOfStore()
        {
            var store = await storeService.SaveAsync(StoreFactory.FakerStore());
            var product = await productService.SaveAsync(ProductFactory.FakerProduct());
            var buyCuanty = store.MaxCapacity * 2;

            var saleBuy = new SaleBuy { Product = product.Id, Store = store.Id, Stock = buyCuanty };

            var response = await controller.BuyProducts(saleBuy);

            var result = response as BadRequestObjectResult;
            Assert.NotNull(result);
            var objectResponse = result.Value as BadResponse;
            Assert.Equal(400, result.StatusCode.Value);
            Assert.Equal(400, objectResponse.Code);
            Assert.Single(objectResponse.Messages);
        }
    }
}
