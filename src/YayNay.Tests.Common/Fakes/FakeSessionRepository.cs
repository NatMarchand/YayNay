using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.Infrastructure;

namespace NatMarchand.YayNay.Tests.Common.Fakes
{
    
    public class FakeSessionRepository : ISessionRepository
    {
        private readonly Dictionary<SessionId, Session> _sessions;

        public IReadOnlyDictionary<SessionId, Session> Sessions => _sessions;

        public FakeSessionRepository()
        {
            _sessions = new Dictionary<SessionId, Session>();
        }

        public Task<Session?> GetAsync(SessionId id)
        {
            return Task.FromResult(Sessions.TryGetValue(id, out var session) ? session : null);
        }

        public Task AddAsync(Session session)
        {
            _sessions.Add(session.Id, session);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Session session)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Session session)
        {
            throw new NotImplementedException();
        }
    }
}