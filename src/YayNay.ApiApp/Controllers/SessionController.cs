using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NatMarchand.YayNay.Core.Domain;
using NatMarchand.YayNay.Core.Domain.Commands.RequestSession;
using NatMarchand.YayNay.Core.Domain.Entities;
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

        [HttpPost("request")]
        public async Task<IActionResult> RequestSessionAsync([FromServices] RequestSessionCommandHandler handler, [FromBody] RequestSessionModel requestSessionModel)
        {
            var command = new RequestSession(
                new List<PersonId>(requestSessionModel.Speakers.Select(g => (PersonId) g)),
                requestSessionModel.Title,
                requestSessionModel.Description,
                requestSessionModel.Tags,
                requestSessionModel.StartTime,
                requestSessionModel.EndTime);

            var (result, events) = await handler.ExecuteAsync(command);
            await _eventDispatcher.DispatchAsync(events);

            return result switch
            {
                SuccessCommandResult src => Accepted(),
                ValidationFailureCommandResult vfcr => this.ValidationProblem(vfcr),
                FailureCommandResult fcr => this.Problem(fcr),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }
    }

#nullable disable
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
}