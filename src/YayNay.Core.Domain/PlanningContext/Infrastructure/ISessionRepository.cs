using System;
using System.Threading.Tasks;
using NatMarchand.YayNay.Core.Domain.Entities;
using NatMarchand.YayNay.Core.Domain.PlanningContext.Entities;

namespace NatMarchand.YayNay.Core.Domain.PlanningContext.Infrastructure
{
    public interface ISessionRepository
    {
        Task<Session?> GetAsync(SessionId id);
        Task AddAsync(Session session);
        Task UpdateAsync(Session session);
        Task DeleteAsync(Session session);
    }
}
