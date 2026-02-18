using Valora.Domain.Common.Interfaces;
using Valora.Domain.Entities;

namespace Valora.Domain.Repositories;

public interface IItemRepository : IRepository<Item>
{
    Task<Item?> GetByNameAsync(string name);
}