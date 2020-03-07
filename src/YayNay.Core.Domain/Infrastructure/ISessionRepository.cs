using System.Threading.Tasks;
using NatMarchand.YayNay.Core.Domain.Entities;

namespace NatMarchand.YayNay.Core.Domain.Infrastructure
{
    public interface ISessionRepository
    {
        Task<Session?> GetAsync(SessionId id);
        Task AddAsync(Session session);
        Task UpdateAsync(Session session);
        Task DeleteAsync(Session session);
    }
}