
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Alachisoft.NCache.Common.Util;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POSsystem.API.Filters;
using POSsystem.Contracts.Constants;
using POSsystem.Contracts.DTO;
using POSsystem.Core.Exceptions;
using POSsystem.Core.Handlers.Commands;
using POSsystem.Core.Handlers.Queries;

namespace POSsystem.Controllers.V1
{
    [Authorize(Roles = $"{UserRoles.Owner},{UserRoles.Admin}")]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ServiceReservationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ServiceReservationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ServiceReservationDTO>), (int)HttpStatusCode.OK)]
        [ProducesErrorResponseType(typeof(BaseResponseDTO))]
        public async Task<IActionResult> Get([FromQuery] int reservationStatus)
        {
            if (reservationStatus == 0)
            {
                var getAllQuery = new GetAllServiceReservationsQuery();
                var getAllResponse = await _mediator.Send(getAllQuery);
                return Ok(getAllResponse);
            }
            
            var getByStatusQuery = new GetServiceReservationsByStatusQuery(reservationStatus);
            var getByStatusReponse = await _mediator.Send(getByStatusQuery);
            return Ok(getByStatusReponse);
        }
        
        [MapToApiVersion("1.0")]
        [HttpGet]
        [Route("{id}")]
        [TypeFilter(typeof(ETagFilter))]
        [ProducesResponseType(typeof(ServiceReservationDTO), (int)HttpStatusCode.OK)]
        [ProducesErrorResponseType(typeof(BaseResponseDTO))]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var query = new GetServiceReservationByIdQuery(id);
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
        [HttpPost]
        [ProducesResponseType(typeof(ServiceReservationDTO), (int) HttpStatusCode.Created)]
        [ProducesErrorResponseType(typeof(BaseResponseDTO))]
        public async Task<IActionResult> Post([FromBody] CreateOrUpdateServiceReservationDTO dto)
        {
            try
            {
                var command = new CreateServiceReservationCommand(dto);
                var response = await _mediator.Send(command);
                return StatusCode((int) HttpStatusCode.Created, response);
            }
            catch (InvalidRequestBodyException exception)
            {
                return BadRequest(new BaseResponseDTO
                {
                    IsSuccess = false,
                    Errors = exception.Errors
                });
            }
        }

        [MapToApiVersion("1.0")]
        [HttpPut]
        [Route("{id}")]
        [TypeFilter(typeof(ETagFilter))]
        [ProducesResponseType(typeof(ServiceReservationDTO), (int) HttpStatusCode.OK)]
        [ProducesErrorResponseType(typeof(BaseResponseDTO))]
        public async Task<IActionResult> Update(int id, [FromBody] CreateOrUpdateServiceReservationDTO model)
        {
            try
            {
                var command = new UpdateServiceReservationCommand(id, model);
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
                    Errors = new[] { ex.Message }
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
            var command = new DeleteServiceReservationCommand(id);
            await _mediator.Send(command);
            return Ok();
        }
    }
}

