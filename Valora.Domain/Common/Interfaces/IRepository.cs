using Valora.Domain.Common.Abstractions;
using Valora.Domain.Common.Results;

namespace Valora.Domain.Common.Interfaces;

public interface IRepository<T> where T : IAggregateRoot
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
    Task<PaginatedList<T>> GetPaginatedAsync(int page, int pageSize, CancellationToken cancellationToken);
}