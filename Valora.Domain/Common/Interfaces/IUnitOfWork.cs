using System.Threading;
using System.Threading.Tasks;

namespace Valora.Domain.Common.Interfaces;

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken = default);
}