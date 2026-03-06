using System;

namespace Valora.Application.UseCases.Users.GetProfile;

public record UserProfileResponse(
    Guid Id,
    string Email,
    string? Nickname,
    DateTimeOffset CreatedAt,
    DateTimeOffset LastLoginAt);
