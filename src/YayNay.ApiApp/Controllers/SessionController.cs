using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.AspNetCore.Routing;
using NatMarchand.YayNay.ApiApp.Identity;
using NatMarchand.YayNay.Core.Domain;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.PlanningContext.Commands.ApproveSession;
using NatMarchand.YayNay.Core.Domain.PlanningContext.Commands.RequestSession;
using NatMarchand.YayNay.Core.Domain.PlanningContext.Commands.ScheduleSession;
using NatMarchand.YayNay.Core.Domain.PlanningContext.Entities;
using NatMarchand.YayNay.Core.Domain.Queries;
using NatMarchand.YayNay.Core.Domain.Queries.Person;
using NatMarchand.YayNay.Core.Domain.Queries.Session;
using NatMarchand.YayNay.Core.Infrastructure.Events;

namespace NatMarchand.YayNay.ApiApp.Controllers
{
    [ApiController]
    [Route("sessions")]
    public class SessionController : ControllerBase
    {
        private readonly EventDispatcher _eventDispatcher;

        public SessionController(EventDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
        }

        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedList<SessionProjection>>> GetSessions([FromQuery] [Required] SessionStatus status, [FromServices] SessionQueries sessionQueries)
        {
            return await sessionQueries.GetSessionsByStatusAsync(status, User.GetProfile());
        }

        [HttpPost("request")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> RequestSessionAsync([FromServices] RequestSessionCommandHandler handler, [FromBody] RequestSessionModel requestSessionModel, CancellationToken cancellationToken)
        {
            var command = new RequestSession(
                new List<PersonId>(requestSessionModel.Speakers.Select(g => (PersonId) g)),
                requestSessionModel.Title,
                requestSessionModel.Description,
                requestSessionModel.Tags,
                requestSessionModel.StartTime,
                requestSessionModel.EndTime);

            var (result, events) = await handler.ExecuteAsync(command, cancellationToken);
            await _eventDispatcher.DispatchAsync(events);

            return result switch
            {
                SuccessCommandResult src => Accepted(),
                ValidationFailureCommandResult vfcr => this.ValidationProblem(vfcr),
                FailureCommandResult fcr => this.Problem(fcr),
                _ => Problem()
            };
        }

        [HttpPost("{sessionId}/approval")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = nameof(UserRight.ApproveSession))]
        public async Task<IActionResult> ApproveSessionAsync([FromServices] ApproveSessionCommandHandler commandHandler, [FromRoute] [Required] Guid sessionId, [FromBody] [Required] ApproveSessionModel approveSessionModel, CancellationToken cancellationToken)
        {
            var command = new ApproveSession(sessionId, User.GetProfile()!, approveSessionModel.IsApproved, approveSessionModel.Comment);
            var (result, events) = await commandHandler.ExecuteAsync(command, cancellationToken);
            await _eventDispatcher.DispatchAsync(events);
            
            return result switch
            {
                SuccessCommandResult scr => Accepted(),
                NotFoundCommandResult nfcr => Problem(statusCode: StatusCodes.Status404NotFound, detail:nfcr.Reason),
                ValidationFailureCommandResult vfcr => this.ValidationProblem(vfcr),
                _ => Problem()
            };
        }
        
        [HttpPost("{sessionId}/schedule")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = nameof(UserRight.ScheduleSession))]
        public async Task<IActionResult> ScheduleSessionAsync([FromServices] ScheduleSessionCommandHandler commandHandler, [FromRoute] [Required] Guid sessionId, [FromBody] [Required] ScheduleSessionModel scheduleSessionModel, CancellationToken cancellationToken)
        {
            var command = new ScheduleSession(sessionId, User.GetProfile()!, scheduleSessionModel.StartTime, scheduleSessionModel.EndTime);
            var (result, events) = await commandHandler.ExecuteAsync(command, cancellationToken);
            await _eventDispatcher.DispatchAsync(events);
            
            return result switch
            {
                SuccessCommandResult scr => Accepted(),
                NotFoundCommandResult nfcr => Problem(statusCode: StatusCodes.Status404NotFound, detail:nfcr.Reason),
                ValidationFailureCommandResult vfcr => this.ValidationProblem(vfcr),
                _ => Problem()
            };
        }
    }

#nullable disable

    public class ApproveSessionModel
    {
        [Required] public bool IsApproved { get; set; }
        [Required] public string Comment { get; set; }
    }

    public class RequestSessionModel
    {
        [Required] public IEnumerable<Guid> Speakers { get; set; }

        [Required, StringLength(64, MinimumLength = 4)]
        public string Title { get; set; }

        [Required, StringLength(short.MaxValue, MinimumLength = 16)]
        public string Description { get; set; }

        [Required] public IEnumerable<string> Tags { get; set; }

        public DateTimeOffset? StartTime { get; set; }

        public DateTimeOffset? EndTime { get; set; }
    }

    public class ScheduleSessionModel
    {
        public DateTimeOffset? StartTime { get; set; }

        public DateTimeOffset? EndTime { get; set; }
    }
}
