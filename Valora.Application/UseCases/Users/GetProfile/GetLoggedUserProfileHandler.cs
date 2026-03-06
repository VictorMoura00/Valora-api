using System.Threading;
using System.Threading.Tasks;
using Valora.Application.Common.Interfaces;
using Valora.Domain.Common.Results;
using Valora.Domain.Repositories;

namespace Valora.Application.UseCases.Users.GetProfile;

public static class GetLoggedUserProfileHandler
{
    public static async Task<Result<UserProfileResponse>> Handle(
        GetLoggedUserProfileQuery query,
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
        {
            return Result<UserProfileResponse>.Failure(Error.Unauthorized(
                "User.NotAuthenticated",
                "Acesso negado."));
        }

        var user = await userRepository.GetByIdAsync(currentUserService.UserId, cancellationToken);

        if (user is null)
        {
            return Result<UserProfileResponse>.Failure(Error.NotFound(
                "User.NotFound",
                "Usuário não encontrado."));
        }

        var response = new UserProfileResponse(
            user.Id,
            user.Email,
            user.Nickname,
            user.CreatedAt,
            user.LastLoginAt
        );

        return Result<UserProfileResponse>.Success(response);
    }
}
