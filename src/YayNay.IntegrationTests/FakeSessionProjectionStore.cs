using System.Collections.Generic;
using System.Threading.Tasks;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Infrastructure.Events;

namespace NatMarchand.YayNay.IntegrationTests
{
    public class FakeSessionProjectionStore : ISessionProjectionStore
    {
        private readonly Dictionary<SessionId, SessionProjection> _projections;
        public IReadOnlyDictionary<SessionId, SessionProjection> Projections => _projections;

        public FakeSessionProjectionStore()
        {
            _projections = new Dictionary<SessionId, SessionProjection>();
        }

        public Task MergeProjectionAsync(SessionProjection projection)
        {
            _projections[projection.Id] = projection;
            return Task.CompletedTask;
        }
    }
}