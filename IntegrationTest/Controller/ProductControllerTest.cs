using AutoMapper;
using IntegrationTest.Config;
using IntegrationTest.Util;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Domain.Model;
using WebApi.Domain.Service;
using WebApi.Infrastructure.Adapter;
using WebApi.Infrastructure.Controllers;
using WebApi.Infrastructure.Dto;
using WebApi.Infrastructure.Dto.Detail;
using Xunit;

namespace IntegrationTest.Controller
{
    public class ProductControllerTest
    {
        private IProductService service = new ProductAdapter(TestDb.GetContext());
        private readonly IMapper mapper;
        private readonly ProductController controller;

        public ProductControllerTest()
        {
            mapper = MapperConfig.GetMapper();
            controller = new ProductController(service, mapper, Serilog.Log.Logger);
        }

        [Fact]
        async Task GetAllOkTest() 
        {
            service = new ProductAdapter(TestDb.GetContext());
            var entitySave = await service.SaveAsync(ProductFactory.FakerProduct());

            var response = await controller.Get();

            var result = response as OkObjectResult;
            var responseObject = result.Value as List<ProductDto>;
            Assert.Equal(200, result.StatusCode.Value);
            Assert.Single(responseObject);
            Assert.Equal(entitySave.Name, responseObject.FirstOrDefault().Name);
            Assert.Equal(entitySave.Id, responseObject.FirstOrDefault().Id);
            Assert.Equal(entitySave.Description, responseObject.FirstOrDefault().Description);
            Assert.Equal(entitySave.Sku, responseObject.FirstOrDefault().Sku);
        }

        [Fact]
        async Task FindByIdOkTest()
        {
            var entitySave = await service.SaveAsync(ProductFactory.FakerProduct());

            var response = await controller.Get(entitySave.Id);

            var result = response as OkObjectResult;
            var responseObject = result.Value as ProductDtoDetail;
            Assert.Equal(200, result.StatusCode.Value);
            Assert.NotNull(responseObject);
            Assert.Equal(entitySave.Name, responseObject.Name);
            Assert.Equal(entitySave.Id, responseObject.Id);
            Assert.Equal(entitySave.Description, responseObject.Description);
            Assert.Equal(entitySave.Sku, responseObject.Sku);
        }

        [Fact]
        async Task FindByIdBadTest()
        {
            var response = await controller.Get(Guid.NewGuid());

            var okResult = response as NotFoundObjectResult;
            Assert.Equal(404, okResult.StatusCode.Value);
            var returnValue = okResult.Value as BadResponse;
            Assert.Equal(404, returnValue.Code);
            Assert.Single(returnValue.Messages);

        }

        [Fact]
        async Task SaveOkTest()
        {
            var response = await controller.Post(mapper.Map<ProductDto>(ProductFactory.FakerProduct()));

            var result = response as OkObjectResult;
            var responseObject = result.Value as Product;
            var entitySave = await service.GetByIdAsync(responseObject.Id);
            Assert.Equal(200, result.StatusCode.Value);
            Assert.NotNull(responseObject);
            Assert.Equal(entitySave.Name, responseObject.Name);
            Assert.Equal(entitySave.Id, responseObject.Id);
            Assert.Equal(entitySave.Description, responseObject.Description);
            Assert.Equal(entitySave.Sku, responseObject.Sku);
        }

        [Fact]
        async Task SaveBadFluentValidationTest()
        {
            var dto = ProductFactory.FluentDtoBad();

            var response = await controller.Post(dto);

            var result = response as BadRequestObjectResult;
            Assert.Equal(400, result.StatusCode.Value);
            var returnValue = result.Value as BadResponse;
            Assert.Equal(400, returnValue.Code);
            Assert.NotEmpty(returnValue.Messages);
        }

        [Fact]
        async Task UpdateBadTest()
        {
            var dto = mapper.Map<ProductDto>(ProductFactory.FakerProduct());

            var response = await controller.Put(Guid.NewGuid(), dto);

            var okResult = response as BadRequestObjectResult;
            Assert.Equal(400, okResult.StatusCode.Value);
            var returnValue = okResult.Value as BadResponse;
            Assert.Equal(400, returnValue.Code);
            Assert.Single(returnValue.Messages);
        }

        [Fact]
        async Task DeleteOkTest()
        {
            var entity = await service.SaveAsync(ProductFactory.FakerProduct());

            var response = await controller.Delete(entity.Id);

            var okResult = response as OkResult;
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        async Task DeleteBadTest()
        {
            var response = await controller.Delete(Guid.NewGuid());

            var okResult = response as BadRequestObjectResult;
            Assert.Equal(400, okResult.StatusCode.Value);
            var returnValue = okResult.Value as BadResponse;
            Assert.Equal(400, returnValue.Code);
            Assert.Single(returnValue.Messages);
        }
    }
}
