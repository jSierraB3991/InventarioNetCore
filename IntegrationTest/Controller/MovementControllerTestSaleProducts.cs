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
    public class MovementControllerTestSaleProducts
    {
        private readonly IMapper mapper;
        private IProductService productService;
        private IStoreService storeService;
        private MovementController controller;

        public MovementControllerTestSaleProducts()
        {
            mapper = MapperConfig.GetMapper();
            var dbContext = TestDb.GetContext();
            productService = new ProductAdapter(dbContext);
            storeService = new StoreAdapter(dbContext);
            var productStoreService = new ProductStoreAdapter(dbContext);
            var movementService = new MovementAdapter(dbContext, productService, storeService, productStoreService);
            controller = new MovementController(movementService, mapper, Serilog.Log.Logger);
        }

        [Fact]
        async Task SaleProductOk()
        {
            var store = await storeService.SaveAsync(StoreFactory.FakerStore());
            var product = await productService.SaveAsync(ProductFactory.FakerProduct());
            var buyCuanty = store.MaxCapacity / 2;
            var buy = new SaleBuy { Product = product.Id, Store = store.Id, Stock = buyCuanty };
            await controller.BuyProducts(buy);


            var saleCuantity = buyCuanty / 2;
            var sale = new SaleBuy { Product = product.Id, Store = store.Id, Stock = saleCuantity };
            var response = await controller.SaleProducts(sale);

            var result = response as OkObjectResult;
            Assert.NotNull(result);
            var objectResponse = result.Value as ProductDtoDetail;
            Assert.Equal(200, result.StatusCode.Value);
            Assert.Equal(buyCuanty - saleCuantity, objectResponse.Stock);
            Assert.Equal(product.Description, objectResponse.Description);
            Assert.Equal(product.Id, objectResponse.Id);
            Assert.Equal(product.Name, objectResponse.Name);
            Assert.Equal(product.Sku, objectResponse.Sku);
            Assert.Equal(store.Name, objectResponse.Stores.FirstOrDefault().Name);
            Assert.Equal(store.Id, objectResponse.Stores.FirstOrDefault().Id);
            Assert.Equal(store.Description, objectResponse.Stores.FirstOrDefault().Description);
            Assert.Equal(store.Direction, objectResponse.Stores.FirstOrDefault().Direction);
            Assert.Equal(store.MaxCapacity, objectResponse.Stores.FirstOrDefault().MaxCapacity);
        }

        [Fact]
        async Task SaleProductBadStoreProduct()
        {
            var store = await storeService.SaveAsync(StoreFactory.FakerStore());
            var product = await productService.SaveAsync(ProductFactory.FakerProduct());
            
            var saleCuantity = store.MaxCapacity / 2;
            var sale = new SaleBuy { Product = product.Id, Store = store.Id, Stock = saleCuantity };
            var response = await controller.SaleProducts(sale);

            var result = response as BadRequestObjectResult;
            Assert.NotNull(result);
            var objectResponse = result.Value as BadResponse;
            Assert.Equal(400, result.StatusCode.Value);
            Assert.Equal(400, objectResponse.Code);
            Assert.Single(objectResponse.Messages);
        }

        [Fact]
        async Task SaleProductBadStockNotFound()
        {
            var store = await storeService.SaveAsync(StoreFactory.FakerStore());
            var product = await productService.SaveAsync(ProductFactory.FakerProduct());

            var buyCuanty = 10;
            var buy = new SaleBuy { Product = product.Id, Store = store.Id, Stock = buyCuanty };
            await controller.BuyProducts(buy);

            var saleCuantity = buyCuanty * 2;
            var sale = new SaleBuy { Product = product.Id, Store = store.Id, Stock = saleCuantity };
            var response = await controller.SaleProducts(sale);

            var result = response as BadRequestObjectResult;
            Assert.NotNull(result);
            var objectResponse = result.Value as BadResponse;
            Assert.Equal(400, result.StatusCode.Value);
            Assert.Equal(400, objectResponse.Code);
            Assert.Single(objectResponse.Messages);
        }
    }
}
