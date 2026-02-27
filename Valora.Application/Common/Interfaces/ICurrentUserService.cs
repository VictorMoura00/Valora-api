using System;

namespace Valora.Application.Common.Interfaces;

public interface ICurrentUserService
{
    /// <summary>
    /// O ID único do usuário extraído do token JWT (Claim 'sub').
    /// Retorna Guid.Empty se o usuário não estiver autenticado.
    /// </summary>
    Guid UserId { get; }

    /// <summary>
    /// Indica se a requisição atual possui um token válido.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// O e-mail do usuário extraído do token JWT.
    /// </summary>
    string? Email { get; }
}
