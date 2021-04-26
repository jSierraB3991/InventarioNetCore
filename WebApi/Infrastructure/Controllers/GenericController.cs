using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Domain.Exceotion;
using WebApi.Domain.Exception;
using WebApi.Domain.Model;
using WebApi.Domain.Service;
using WebApi.Infrastructure.Dto;

namespace WebApi.Infrastructure.Controllers
{
    public abstract class GenericController<Rest, Entity, Detail> : ControllerBase where Entity : AuditableEntity where Rest : BaseDto where Detail : Rest
    {
        private readonly IGenericService<Entity> _service;
        private readonly IMapper mapper;
        private readonly ILogger logger;
        private readonly AbstractValidator<Rest> fluentValidation;

        public GenericController(IGenericService<Entity> service, IMapper mapper, ILogger logger, AbstractValidator<Rest> fluentValidation)
        {
            _service = service;
            this.mapper = mapper;
            this.logger = logger;
            this.fluentValidation = fluentValidation;
        }

        // GET: api/<GenericController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                List<Entity> list = await _service.GetListAsync();
                var listDto = mapper.Map<List<Rest>>(list);

                return Ok(listDto);
            }
            catch (BusinessException ex)
            {
                logger.Warning(ex.Message);
                return BadRequest(new BadResponse { Code = 400, Messages = new string[] { ex.Message } });
            }
        }

        // GET api/<GenericController>/03b6196b-bfa6-4b71-8708-323f2d11c37b
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(System.Guid id)
        {
            try
            {
                Entity entity = await _service.FindByIdAllData(id);
                return Ok(mapper.Map<Detail>(entity));
            }
            catch (BusinessException ex)
            {
                logger.Warning(ex.Message);
                return NotFound(new BadResponse { Code = 404, Messages = new string[] { ex.Message } });
            }
        }

        // POST api/<GenericController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Rest rest)
        {
            try
            {
                FluentValidation(rest);
                Entity entity = await _service.SaveAsync(mapper.Map<Entity>(rest));
                return Ok(entity);
            }
            catch (BusinessException ex)
            {
                logger.Warning(ex.Message);
                return BadRequest(new BadResponse { Code = 400, Messages = new string[] { ex.Message } });
            }
            catch (DuplicateDataException ex)
            {
                logger.Warning(ex.Message);
                return Conflict(new BadResponse { Code = 409, Messages = new string[] { ex.Message } });
            }
            catch (FluentValidationException ex)
            {
                logger.Warning(JsonConvert.SerializeObject(ex.Messages));
                return BadRequest(new BadResponse { Code = 400, Messages = ex.Messages });
            }
        }

        // PUT api/<GenericController>/03b6196b-bfa6-4b71-8708-323f2d11c37b
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(System.Guid id, [FromBody] Rest rest)
        {
            try
            {
                FluentValidation(rest);
                await _service.UpdateAsync(id, mapper.Map<Entity>(rest));
                rest.Id = id;
                return Ok(rest);
            }
            catch (BusinessException ex)
            {
                logger.Warning(ex.Message);
                return BadRequest(new BadResponse { Code = 400, Messages = new string[] { ex.Message } });
            }
            catch (DuplicateDataException ex)
            {
                logger.Warning(ex.Message);
                return Conflict(new BadResponse { Code = 409, Messages = new string[] { ex.Message } });
            }
            catch (FluentValidationException ex)
            {
                logger.Warning(JsonConvert.SerializeObject(ex.Messages));
                return BadRequest(new BadResponse { Code = 400, Messages = ex.Messages });
            }
        }

        // DELETE api/<GenericController>/03b6196b-bfa6-4b71-8708-323f2d11c37b
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(System.Guid id)
        {
            try
            {
                await _service.DeleteByidAync(id);
                return Ok();
            }
            catch (BusinessException ex)
            {
                logger.Warning(ex.Message);
                return BadRequest(new BadResponse { Code = 400, Messages = new string[] { ex.Message } });
            }
        }


        private void FluentValidation(Rest rest)
        {
            FluentValidation.Results.ValidationResult validationResult = fluentValidation.Validate(rest);
            if (!validationResult.IsValid)
            {
                throw new FluentValidationException(validationResult.Errors.Select(error => error.ErrorMessage).ToArray());
            }
        }
    }
}
