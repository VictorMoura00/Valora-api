using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Valora.Application.Common.Interfaces;

namespace Valora.Api.Service;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public Guid UserId
    {
        get
        {
            // Busca a claim "NameIdentifier" (padrão do .NET) ou a claim "sub" (padrão nativo do OAuth2)
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                     ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub");

            if (claim != null && Guid.TryParse(claim.Value, out var userId))
            {
                return userId;
            }

            return Guid.Empty;
        }
    }

    public string? Email =>
        _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value
     ?? _httpContextAccessor.HttpContext?.User?.FindFirst("email")?.Value;
}
