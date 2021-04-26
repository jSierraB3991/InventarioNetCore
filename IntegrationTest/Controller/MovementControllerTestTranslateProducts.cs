using AutoMapper;
using IntegrationTest.Config;
using IntegrationTest.Util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Domain.Model;
using WebApi.Domain.Service;
using WebApi.Infrastructure.Adapter;
using WebApi.Infrastructure.Controllers;
using WebApi.Infrastructure.Dto;
using WebApi.Infrastructure.Dto.Request;
using Xunit;

namespace IntegrationTest.Controller
{
    public class MovementControllerTestTranslateProducts
    {
        private readonly IMapper mapper;
        private readonly IProductService productService;
        private readonly IStoreService storeService;
        private readonly MovementController controller;

        public MovementControllerTestTranslateProducts()
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
        private async Task TranslateProductOfStoreOk()
        {
            var dest = await storeService.SaveAsync(StoreFactory.FakerStore());

            var origin = await storeService.SaveAsync(StoreFactory.FakerStore());
            var product = await productService.SaveAsync(ProductFactory.FakerProduct());
            var buyCuanty = origin.MaxCapacity / 2;
            var buy = new SaleBuy { Product = product.Id, Store = origin.Id, Stock = buyCuanty };
            await controller.BuyProducts(buy);

            var productTranslate = buyCuanty / 2;
            var translateRequest = new TranslateRequest
            {
                Origin = origin.Id,
                Destino = dest.Id,
                Product = product.Id,
                Stock = productTranslate
            };
            var response = await controller.TranslateProductStore(translateRequest);

            var result = response as OkResult;
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        private async Task TranslateProductBadStockNotFound()
        {
            var dest = await storeService.SaveAsync(StoreFactory.FakerStore());
            var origin = await storeService.SaveAsync(StoreFactory.FakerStore());
            var product = await productService.SaveAsync(ProductFactory.FakerProduct());

            var productTranslate = 10;
            var translateRequest = new TranslateRequest
            {
                Origin = origin.Id,
                Destino = dest.Id,
                Product = product.Id,
                Stock = productTranslate
            };
            var response = await controller.TranslateProductStore(translateRequest);

            var result = response as BadRequestObjectResult;
            Assert.NotNull(result);
            var objectResponse = result.Value as BadResponse;
            Assert.Equal(400, result.StatusCode.Value);
            Assert.Equal(400, objectResponse.Code);
            Assert.Single(objectResponse.Messages);
        }

        [Fact]
        private async Task TranslateProductBadMaximunProduct()
        {
            var maximun = 0;
            var storesOrigin = new List<Store>();
            var product = await productService.SaveAsync(ProductFactory.FakerProduct());
            while (maximun < 10000)
            {
                Store origin = await storeService.SaveAsync(StoreFactory.FakerStore());
                maximun += origin.MaxCapacity;
                if (maximun < 10000)
                {
                    storesOrigin.Add(origin);
                }
            }

            storesOrigin.ForEach(async origin =>
            {
                SaleBuy buy = new SaleBuy { Product = product.Id, Store = origin.Id, Stock = origin.MaxCapacity };
                await controller.BuyProducts(buy);
            });
            var capacityThrow = (10000 - storesOrigin.Sum(store => store.MaxCapacity)) + Faker.RandomNumber.Next(10, 1000);
            var entity = StoreFactory.FakerStore();
            entity.MaxCapacity = capacityThrow;
            var originThrow = await storeService.SaveAsync(entity);
            var dest = await storeService.SaveAsync(StoreFactory.FakerStore());
            var translateRequest = new TranslateRequest
            {
                Product = product.Id,
                Origin = originThrow.Id,
                Stock = originThrow.MaxCapacity,
                Destino = dest.Id
            };
            var response = await controller.TranslateProductStore(translateRequest);

            var result = response as BadRequestObjectResult;
            Assert.NotNull(result);
            var objectResponse = result.Value as BadResponse;
            Assert.Equal(400, result.StatusCode.Value);
            Assert.Equal(400, objectResponse.Code);
            Assert.Single(objectResponse.Messages);
        }
    }
}
