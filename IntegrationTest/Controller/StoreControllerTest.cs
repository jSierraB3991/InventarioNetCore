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
    public class StoreControllerTest
    {
        private IStoreService service = new StoreAdapter(TestDb.GetContext());
        private readonly IMapper mapper;
        private readonly StoreController controller;

        public StoreControllerTest()
        {
            mapper = MapperConfig.GetMapper();
            controller = new StoreController(service, mapper, Serilog.Log.Logger);
        }

        [Fact]
        async Task GetAllOkTest()
        {
            service = new StoreAdapter(TestDb.GetContext());
            var store = await service.SaveAsync(StoreFactory.FakerStore());
            
            var response = await controller.Get();
            
            var okResult = response as OkObjectResult;
            Assert.Equal(200, okResult.StatusCode.Value);
            var returnsValue = okResult.Value as List<StoreDto>;
            Assert.Single(returnsValue);
            Assert.Equal(returnsValue.FirstOrDefault().Id, store.Id);
            Assert.Equal(returnsValue.FirstOrDefault().Description, store.Description);
            Assert.Equal(returnsValue.FirstOrDefault().Direction, store.Direction);
            Assert.Equal(returnsValue.FirstOrDefault().MaxCapacity, store.MaxCapacity);
            Assert.Equal(returnsValue.FirstOrDefault().Name, store.Name);
        }

        [Fact]
        async Task FindByIdOkTest() 
        {
            var store = await service.SaveAsync(StoreFactory.FakerStore());
            
            var response = await controller.Get(store.Id);

            var okResult = response as OkObjectResult;
            Assert.Equal(200, okResult.StatusCode.Value);
            var returnValue = okResult.Value as StoreDtoDetail;
            Assert.NotNull(returnValue);
            Assert.Equal(returnValue.MaxCapacity, store.MaxCapacity);
            Assert.Equal(returnValue.Name, store.Name);
            Assert.Equal(returnValue.Description, store.Description);
            Assert.Equal(returnValue.Direction, store.Direction);
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
            var dto = mapper.Map<StoreDto>(StoreFactory.FakerStore());

            var response = await controller.Post(dto);

            var okResult = response as OkObjectResult;
            Assert.Equal(200, okResult.StatusCode.Value);
            var returnValue = okResult.Value as Store;
            Assert.NotNull(returnValue);
            var data = await service.GetByIdAsync(returnValue.Id);
            Assert.Equal(returnValue.Description, data.Description);
            Assert.Equal(returnValue.Direction, data.Direction);
            Assert.Equal(returnValue.MaxCapacity, data.MaxCapacity);
            Assert.Equal(returnValue.Name, data.Name);
        }

        [Fact]
        async Task SaveBadFluentValidationTest()
        {
            var dto = StoreFactory.FluentDtoBad();
            
            var response = await controller.Post(dto);

            var result = response as BadRequestObjectResult;
            Assert.Equal(400, result.StatusCode.Value);
            var returnValue = result.Value as BadResponse;
            Assert.Equal(400, returnValue.Code);
            Assert.NotEmpty(returnValue.Messages);
        }

        async Task SaveBadNameDuplicateTest()
        {
            var entity = await service.SaveAsync(StoreFactory.FakerStore());
            var dto = mapper.Map<StoreDto>(StoreFactory.FakerStore());
            dto.Name = entity.Name;

            var response = await controller.Post(dto);

            var result = response as ConflictObjectResult;
            Assert.Equal(409, result.StatusCode.Value);
            var returnValue = result.Value as BadResponse;
            Assert.Equal(409, returnValue.Code);
            Assert.Single(returnValue.Messages);
            Assert.Contains(returnValue.Messages.FirstOrDefault(), entity.Name);
        }

        [Fact]
        async Task UpdateBadTest()
        {
            var dto = mapper.Map<StoreDto>(StoreFactory.FakerStore());
            
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
            var entity = await service.SaveAsync(StoreFactory.FakerStore());

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
