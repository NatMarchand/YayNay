using System;
using System.Threading.Tasks;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.PlanningContext.Entities;
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
            switch (status)
            {
                case SessionStatus.Requested when requester?.HasRight(UserRight.ApproveSession) == true:
                case SessionStatus.Approved when requester?.HasRight(UserRight.ScheduleSession) == true:
                case SessionStatus.Scheduled:
                    return await _sessionProjectionStore.GetSessionsAsync(status);
                default:
                    return PagedList<SessionProjection>.Empty;
            }
        }
    }
}
