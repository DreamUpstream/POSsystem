using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using POSsystem.API.Filters;
using POSsystem.Contracts.Constants;
using POSsystem.Contracts.DTO;
using POSsystem.Core.Exceptions;
using POSsystem.Core.Handlers.Commands;
using POSsystem.Core.Handlers.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace POSsystem.API.Controllers.V1
{
    [Authorize(Roles = $"{UserRoles.Owner},{UserRoles.Admin}")]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [MapToApiVersion("1.0")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CustomerDTO>), (int)HttpStatusCode.OK)]
        [ProducesErrorResponseType(typeof(BaseResponseDTO))]
        public async Task<IActionResult> Get()
        {
            var query = new GetAllCustomersQuery();
            var response = await _mediator.Send(query);
            return Ok(response);
        }
        
        [MapToApiVersion("1.0")]
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(CustomerDTO), (int)HttpStatusCode.OK)]
        [ProducesErrorResponseType(typeof(BaseResponseDTO))]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var query = new GetCustomerByIdQuery(id);
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new BaseResponseDTO
                {
                    IsSuccess = false,
                    Errors = new string[] { ex.Message }
                });
            }
        }
        [MapToApiVersion("1.0")]
        [HttpPost, Route ("create")]
        [ProducesResponseType(typeof(CustomerDTO), (int)HttpStatusCode.Created)]
        [ProducesErrorResponseType(typeof(BaseResponseDTO))]
        public async Task<IActionResult> Post([FromBody] CreateOrUpdateCustomerDTO model)
        {
            try
            {

                var command = new CreateCustomerCommand(model);
                var response = await _mediator.Send(command);
                return StatusCode((int)HttpStatusCode.Created, response);
            }
            catch (InvalidRequestBodyException ex)
            {
                return BadRequest(new BaseResponseDTO
                {
                    IsSuccess = false,
                    Errors = ex.Errors
                });
            }
        }
        [MapToApiVersion("1.0")]
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesErrorResponseType(typeof(BaseResponseDTO))]
        public async Task<IActionResult> Delete(int id)
        {
            try 
            {
                var command = new DeleteCustomerCommand(id);
                var deletedId = await _mediator.Send(command);
            }
            catch (InvalidRequestBodyException ex)
            {
                return BadRequest(new BaseResponseDTO
                {
                    IsSuccess = false,
                    Errors = ex.Errors
                });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new BaseResponseDTO
                {
                    IsSuccess = false,
                    Errors = new[] { ex.Message }
                });
            }
            return Ok();
        }
        
        [MapToApiVersion("1.0")]
        [HttpPut]
        [Route("{id}/update")]
        [ProducesResponseType(typeof(CreateOrUpdateCustomerDTO), (int)HttpStatusCode.OK)]
        [ProducesErrorResponseType(typeof(BaseResponseDTO))]
        public async Task<IActionResult> Update(int id, [FromBody] CreateOrUpdateCustomerDTO model)
        {
            try
            {
                var command = new UpdateCustomerCommand(model, id);
                var response = await _mediator.Send(command);
                return Ok(response);
            }
            catch (InvalidRequestBodyException ex)
            {
                return BadRequest(new BaseResponseDTO
                {
                    IsSuccess = false,
                    Errors = ex.Errors
                });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new BaseResponseDTO
                {
                    IsSuccess = false,
                    Errors = new string[] { ex.Message }
                });
            }
        }
    }
    
 
}
