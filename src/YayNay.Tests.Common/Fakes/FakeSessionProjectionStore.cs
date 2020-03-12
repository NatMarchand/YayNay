using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.Queries;
using NatMarchand.YayNay.Core.Domain.Queries.Session;

namespace NatMarchand.YayNay.Tests.Common.Fakes
{
    public class FakeSessionProjectionStore : ISessionProjectionStore
    {
        private readonly Dictionary<SessionId, SessionProjection> _projections;
        public IReadOnlyDictionary<SessionId, SessionProjection> Projections => _projections;

        public FakeSessionProjectionStore()
        {
            _projections = new Dictionary<SessionId, SessionProjection>();
        }

        public Task<PagedList<SessionProjection>> GetSessionsAsync(SessionStatus status)
        {
            return Task.FromResult(new PagedList<SessionProjection>(_projections.Values.Where(s => s.Status == status).ToList(), new Paging(0, 1)));
        }

        public Task MergeProjectionAsync(SessionProjection projection)
        {
            _projections[projection.Id] = projection;
            return Task.CompletedTask;
        }
    }
}