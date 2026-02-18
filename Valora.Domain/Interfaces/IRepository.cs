using Valora.Domain.Common;
using Valora.Domain.Common.Abstractions;

namespace Valora.Domain.Interfaces;

public interface IRepository<T> where T : IAggregateRoot
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}