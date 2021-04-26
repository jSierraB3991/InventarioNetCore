using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Domain.enums;
using WebApi.Domain.Exceotion;
using WebApi.Domain.Exception;
using WebApi.Domain.Service;
using WebApi.Infrastructure.Dto;
using WebApi.Infrastructure.Dto.Detail;
using WebApi.Infrastructure.Dto.Request;
using WebApi.Infrastructure.Validation;

namespace WebApi.Infrastructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovementController : ControllerBase
    {
        private readonly IMovementService service;
        private readonly IMapper mapper;
        private readonly ILogger logger;

        public MovementController(IMovementService service, IMapper mapper, ILogger logger)
        {
            this.service = service;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpPost]
        [Route("/sale")]
        public async Task<IActionResult> SaleProducts([FromBody] SaleBuy saleBuys)
        {
            try
            {
                FluentValidation(saleBuys, new SaleBuyValidation());
                var products = await service.SaleOrBuyProducts(saleBuys, MovementType.OUTPUT);
                return Ok(mapper.Map<ProductDtoDetail>(products));
            }
            catch (BusinessException ex)
            {
                logger.Warning(ex.Message);
                return BadRequest(new BadResponse { Code = 400, Messages = new string[] { ex.Message } });
            }
            catch (FluentValidationException ex)
            {
                logger.Warning(JsonConvert.SerializeObject(ex.Messages));
                return BadRequest(new BadResponse { Code = 400, Messages = ex.Messages });
            }
        }

        [HttpPost]
        [Route("/buy")]
        public async Task<IActionResult> BuyProducts([FromBody] SaleBuy saleBuys)
        {
            try
            {
                FluentValidation(saleBuys, new SaleBuyValidation());
                var products = await service.SaleOrBuyProducts(saleBuys, MovementType.INPUT);
                return Ok(mapper.Map<ProductDtoDetail>(products));
            }
            catch (BusinessException ex)
            {
                logger.Warning(ex.Message);
                return BadRequest(new BadResponse { Code = 400, Messages = new string[] { ex.Message } });
            }
            catch (FluentValidationException ex)
            {
                logger.Warning(JsonConvert.SerializeObject(ex.Messages));
                return BadRequest(new BadResponse { Code = 400, Messages = ex.Messages });
            }
        }

        [HttpPost]
        [Route("/translate")]
        public async Task<IActionResult> TranslateProductStore([FromBody] TranslateRequest translate)
        {
            try
            {
                FluentValidation(translate, new TranslateValidation());
                await service.TranslateProductStore(translate);
                return Ok();
            }
            catch (BusinessException ex)
            {
                logger.Warning(ex.Message);
                return BadRequest(new BadResponse { Code = 400, Messages = new string[] { ex.Message } });
            }
            catch (FluentValidationException ex)
            {
                logger.Warning(JsonConvert.SerializeObject(ex.Messages));
                return BadRequest(new BadResponse { Code = 400, Messages = ex.Messages });
            }
        }

        private static void FluentValidation<Rest>(Rest rest, AbstractValidator<Rest> fluentValidator)
        {
            var validations = fluentValidator.Validate(rest);
            if (!validations.IsValid)
            {
                throw new FluentValidationException(validations.Errors.Select(error => error.ErrorMessage).ToArray());
            }
        }
    }
}
