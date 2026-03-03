using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Valora.Application.Common.Interfaces;

namespace Valora.Api.Service;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public Guid UserId
    {
        get
        {
            // Busca a claim "NameIdentifier" (padrão do .NET) ou a claim "sub" (padrão nativo do OAuth2)
            var claim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                     ?? httpContextAccessor.HttpContext?.User?.FindFirst("sub");

            if (claim != null && Guid.TryParse(claim.Value, out var userId))
            {
                return userId;
            }

            return Guid.Empty;
        }
    }

    public string? Email =>
        httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value
     ?? httpContextAccessor.HttpContext?.User?.FindFirst("email")?.Value;
}
