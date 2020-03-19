using System;
using System.Threading.Tasks;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.PlanningContext.Entities;

namespace NatMarchand.YayNay.Core.Domain.Queries.Session
{
    public interface ISessionProjectionStore
    {
        Task MergeProjectionAsync(SessionProjection projection);
        Task<PagedList<SessionProjection>> GetSessionsAsync(SessionStatus status);
    }
}