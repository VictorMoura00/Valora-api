using Valora.Domain.Common.Interfaces;
using Valora.Domain.Entities;

namespace Valora.Domain.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByNameAsync(string name);
}