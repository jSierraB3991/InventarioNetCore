using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using WebApi.Domain.Model;
using WebApi.Domain.Service;
using WebApi.Infrastructure.Dto;
using WebApi.Infrastructure.Dto.Detail;
using WebApi.Infrastructure.Validation;

namespace WebApi.Infrastructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : GenericController<StoreDto, Store, StoreDtoDetail>
    {
        public StoreController(IStoreService service, IMapper mapper, ILogger logger) : base(service, mapper, logger, new StoreValidation())
        {

        }
    }
}
