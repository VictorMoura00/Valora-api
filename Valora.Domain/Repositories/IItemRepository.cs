using Valora.Domain.Common.Interfaces;
using Valora.Domain.Common.Pagination;
using Valora.Domain.Entities;

namespace Valora.Domain.Repositories;

public interface IItemRepository : IRepository<Item>
{
    Task<bool> ExistsByNameAsync(
        string name,
        Guid categoryId,
        CancellationToken cancellationToken = default
        );
    Task<PaginatedList<Item>> GetPaginatedByCategoryAsync(
Guid categoryId,
        int page, int pageSize,
        CancellationToken cancellationToken = default
        );

    Task<PaginatedList<Item>> SearchByNameAsync(
        string searchTerm,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
