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
    public class ProductController : GenericController<ProductDto, Product, ProductDtoDetail>
    {
        public ProductController(IProductService service, IMapper mapper, ILogger logger) : base(service, mapper, logger, new ProductValidation())
        {

        }
    }
}
