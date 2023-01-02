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
    public class DiscountsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DiscountsController(IMediator mediator) => _mediator = mediator;

        [MapToApiVersion("1.0")]
        [HttpPost]
        [Route("create")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesErrorResponseType(typeof(BaseResponseDTO))]
        public async Task<IActionResult> Create([FromBody] DiscountDTO model)
        {
            try
            {
                var command = new CreateDiscountCommand(model);
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
        [ProducesResponseType(typeof(IEnumerable<EmployeeDTO>), (int)HttpStatusCode.OK)]
        [ProducesErrorResponseType(typeof(BaseResponseDTO))]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllDiscountsQuery();
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        [Route("{id}/get")]
        [ProducesResponseType(typeof(EmployeeDTO), (int)HttpStatusCode.OK)]
        [ProducesErrorResponseType(typeof(BaseResponseDTO))]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var query = new GetDiscountByIdQuery(id);
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
        public async Task<IActionResult> Update(int id, [FromBody] DiscountDTO model)
        {
            try
            {
                var command = new UpdateDiscountCommand(id, model);
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
            var command = new DeleteDiscountCommand(id);
            await _mediator.Send(command);
            return Ok();
        }

        [MapToApiVersion("1.0")]
        [HttpPost]
        [Route("{id}/promote")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesErrorResponseType(typeof(BaseResponseDTO))]
        public IActionResult Promote(int id)
        {
            // no real promote logic can be implemented without front-end?
            return Ok("Promoted.");
        }

    }
}