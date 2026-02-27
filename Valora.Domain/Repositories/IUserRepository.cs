using System;
using System.Threading;
using System.Threading.Tasks;
using Valora.Domain.Common.Interfaces;
using Valora.Domain.Entities;

namespace Valora.Domain.Repositories;

public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Verifica se um nickname já está em uso por outro usuário ativo.
    /// Útil para validação no momento em que o usuário tenta definir ou alterar seu perfil.
    /// </summary>
    /// <param name="nickname">O nickname desejado.</param>
    /// <param name="excludeUserId">O ID do usuário atual (para não dar falso positivo caso ele salve o próprio nome).</param>
    Task<bool> IsNicknameTakenAsync(string nickname, Guid? excludeUserId = null, CancellationToken cancellationToken = default);
}
