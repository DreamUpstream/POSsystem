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
using POSsystem.Contracts.Data.Entities;

namespace POSsystem.Controllers.V1
{
    [Authorize(Roles = $"{UserRoles.Owner},{UserRoles.Admin}")]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PurchasesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PurchasesController(IMediator mediator) => _mediator = mediator;

        [MapToApiVersion("1.0")]
        [HttpPost]
        [Route("create")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesErrorResponseType(typeof(BaseResponseDTO))]
        public async Task<IActionResult> Create([FromBody] OrderDTO model)
        {
            try
            {
                var command = new CreateOrderCommand(model);
                var response = await _mediator.Send(command);
                return StatusCode((int)HttpStatusCode.Created);
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
        [HttpGet]
        [Route("get")]
        [ProducesResponseType(typeof(IEnumerable<OrderDTO>), (int)HttpStatusCode.OK)]
        [ProducesErrorResponseType(typeof(BaseResponseDTO))]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllOrdersQuery();
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        [Route("get/ActiveOrder")]
        [ProducesResponseType(typeof(IEnumerable<OrderDTO>), (int)HttpStatusCode.OK)]
        [ProducesErrorResponseType(typeof(BaseResponseDTO))]
        public async Task<IActionResult> GetAllActive()
        {
            var query = new GetAllActiveOrdersQuery();
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        [Route("get/AvailableItem")]
        [ProducesResponseType(typeof(IEnumerable<OrderDTO>), (int)HttpStatusCode.OK)]
        [ProducesErrorResponseType(typeof(BaseResponseDTO))]
        public async Task<IActionResult> GetAllAvailableItems()
        {
            var query = new GetAllAvailableItemsQuery();
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        [Route("get/AvailableItem")]
        [ProducesResponseType(typeof(IEnumerable<ServiceDTO>), (int)HttpStatusCode.OK)]
        [ProducesErrorResponseType(typeof(BaseResponseDTO))]
        public async Task<IActionResult> GetAllAvailableServices()
        {
            var query = new GetAllAvailableServicesQuery();
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        [Route("{id}/get")]
        [ProducesResponseType(typeof(OrderDTO), (int)HttpStatusCode.OK)]
        [ProducesErrorResponseType(typeof(BaseResponseDTO))]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var query = new GetOrderByIdQuery(id);
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
        [HttpPut]
        [Route("{id}/update")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesErrorResponseType(typeof(BaseResponseDTO))]
        public async Task<IActionResult> Update(int id, [FromBody] OrderDTO model)
        {
            try
            {
                var command = new UpdateOrderCommand(id, model);
                var response = await _mediator.Send(command);
                return StatusCode((int)HttpStatusCode.OK);
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
        [HttpPut]
        [Route("{id}/addProduct")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesErrorResponseType(typeof(BaseResponseDTO))]
        public async Task<IActionResult> AddItem(int id, [FromBody] AddItemDTO model)
        {
            try
            {
                var command = new AddItemToOrderCommand(id, model);
                var response = await _mediator.Send(command);
                return StatusCode((int)HttpStatusCode.OK);
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
        [HttpPut]
        [Route("{id}/addService")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesErrorResponseType(typeof(BaseResponseDTO))]
        public async Task<IActionResult> AddService(int id, [FromBody] AddServiceDTO model)
        {
            try
            {
                var command = new AddServiceToOrderCommand(id, model);
                var response = await _mediator.Send(command);
                return StatusCode((int)HttpStatusCode.OK);
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
        [HttpPut]
        [Route("{id}/Cancel")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesErrorResponseType(typeof(BaseResponseDTO))]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var command = new CancelOrderCommand(id);
                var response = await _mediator.Send(command);
                return StatusCode((int)HttpStatusCode.OK);
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
        [Route("{id}/delete")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesErrorResponseType(typeof(BaseResponseDTO))]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteOrderCommand(id);
            await _mediator.Send(command);
            return Ok();
        }

    }
}