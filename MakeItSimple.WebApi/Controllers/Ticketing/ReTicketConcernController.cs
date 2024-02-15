﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket.AddNewReTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket.ApproveReTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket.GetReTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ReTicket.UpsertReTicket;

namespace MakeItSimple.WebApi.Controllers.Ticketing
{
    [Route("api/re-ticket")]
    [ApiController]
    public class ReTicketConcernController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReTicketConcernController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddNewReTicket([FromBody] AddNewReTicketCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Added_By = userId;
                }
                var results = await _mediator.Send(command);
                if (results.IsFailure)
                {
                    return BadRequest(results);
                }
                return Ok(results);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("page")]
        public async Task<IActionResult> GetReTicket([FromQuery] GetReTicketQuery query)
        {
            try
            {
                var ticketRequest = await _mediator.Send(query);

                Response.AddPaginationHeader(

                ticketRequest.CurrentPage,
                ticketRequest.PageSize,
                ticketRequest.TotalCount,
                ticketRequest.TotalPages,
                ticketRequest.HasPreviousPage,
                ticketRequest.HasNextPage

                );

                var result = new
                {
                    ticketRequest,
                    ticketRequest.CurrentPage,
                    ticketRequest.PageSize,
                    ticketRequest.TotalCount,
                    ticketRequest.TotalPages,
                    ticketRequest.HasPreviousPage,
                    ticketRequest.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }

        [HttpPut("approval")]
        public async Task<IActionResult> ApproveReTicket([FromBody] ApproveReTicketCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Re_Ticket_By = userId;
                }
                var results = await _mediator.Send(command);
                if(results.IsFailure)
                {
                    return BadRequest(results);
                }
                return Ok(results);

            }
            catch(Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("upsert/{id}")]
        public async Task<IActionResult> UpsertReTicket([FromBody] UpsertReTicketCommand command ,[FromRoute] int id)
        {
            try
            {
                command.RequestGeneratorId = id;
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Modified_By = userId;
                    command.Added_By = userId;
                }
                var results = await _mediator.Send(command);
                if (results.IsFailure)
                {
                    return BadRequest(results);
                }
                return Ok(results);

            }
            catch(Exception ex)
            {
                return Conflict(ex.Message);
            }
        }
 
    }
}
