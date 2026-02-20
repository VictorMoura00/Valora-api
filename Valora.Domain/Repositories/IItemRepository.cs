using Valora.Domain.Common.Interfaces;
using Valora.Domain.Common.Pagination;
using Valora.Domain.Entities;

namespace Valora.Domain.Repositories;

public interface IItemRepository : IRepository<Item>
{
    Task<PaginatedList<Item>> GetByCategoryAsync(
        Guid? categoryId, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken
    );
}