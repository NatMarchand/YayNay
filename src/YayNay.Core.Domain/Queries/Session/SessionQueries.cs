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
            if (requester == null || !requester.HasRight(UserRight.AcceptSession))
            {
                return new PagedList<SessionProjection>(Array.Empty<SessionProjection>());
            }

            return await _sessionProjectionStore.GetSessionsAsync(status);
        }
    }
}