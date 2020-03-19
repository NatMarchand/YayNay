using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.PlanningContext.Entities;
using NatMarchand.YayNay.Core.Domain.PlanningContext.Infrastructure;
using NatMarchand.YayNay.Core.Domain.Queries;
using NatMarchand.YayNay.Core.Domain.Queries.Person;
using NatMarchand.YayNay.Core.Domain.Queries.Session;

namespace NatMarchand.YayNay.Tests.Common.Fakes
{
    public class FakeSessionStore : ISessionRepository, ISessionProjectionStore
    {
        private readonly Dictionary<SessionId, Session> _sessions;
        private readonly Dictionary<SessionId, SessionProjection> _projections;

        public IReadOnlyDictionary<SessionId, Session> Sessions => _sessions;
        public IReadOnlyDictionary<SessionId, SessionProjection> Projections => _projections;

        public FakeSessionStore()
        {
            _sessions = new Dictionary<SessionId, Session>();
            _projections = new Dictionary<SessionId, SessionProjection>();
        }

        public Task<Session?> GetAsync(SessionId id)
        {
            if (Sessions.TryGetValue(id, out var session))
            {
                var m = typeof(Session).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
                session = (Session) m.Invoke(session, new object[0]);

                return Task.FromResult<Session?>(session);
            }

            return Task.FromResult(default(Session?));
        }

        public Task AddAsync(Session session)
        {
            _sessions.Add(session.Id, session);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Session session)
        {
            _sessions[session.Id] = session;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Session session)
        {
            _sessions.Remove(session.Id);
            return Task.CompletedTask;
        }

        public Task MergeProjectionAsync(SessionProjection projection)
        {
            _projections[projection.Id] = projection;
            return Task.CompletedTask;
        }

        public Task<PagedList<SessionProjection>> GetSessionsAsync(SessionStatus status)
        {
            return Task.FromResult(new PagedList<SessionProjection>(_projections.Values.Where(s => s.Status == status).ToList(), new Paging(0, 1)));
        }

        public void AddSession(Session session)
        {
            _sessions.Add(session.Id, session);
            _projections.Add(session.Id, new SessionProjection(session.Id, session.Title, session.Description, session.Schedule, session.Status, session.Tags, Array.Empty<PersonName>()));
        }
    }
}
