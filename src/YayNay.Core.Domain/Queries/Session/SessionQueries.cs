using System;
using System.Threading.Tasks;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.Queries.Person;

namespace NatMarchand.YayNay.Core.Domain.Queries.Session
{
    public class SessionQueries
    {
        private readonly ISessionProjectionStore _sessionProjectionStore;

        public SessionQueries(ISessionProjectionStore sessionProjectionStore)
        {
            _sessionProjectionStore = sessionProjectionStore;
        }

        public async Task<PagedList<SessionProjection>> GetSessionsByStatusAsync(SessionStatus status, PersonProfile? requester)
        {
            return status switch
            {
                _ when requester == null => PagedList<SessionProjection>.Empty,
                SessionStatus.Requested when requester.HasRight(UserRight.ApproveSession) => await _sessionProjectionStore.GetSessionsAsync(status),
                SessionStatus.Approved when requester.HasRight(UserRight.ScheduleSession) => await _sessionProjectionStore.GetSessionsAsync(status),
                _ => PagedList<SessionProjection>.Empty 
            };
        }
    }
}
