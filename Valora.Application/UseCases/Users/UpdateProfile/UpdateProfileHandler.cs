using System.Threading;
using System.Threading.Tasks;
using Valora.Application.Common.Interfaces;
using Valora.Domain.Common.Interfaces;
using Valora.Domain.Common.Results;
using Valora.Domain.Repositories;

namespace Valora.Application.UseCases.Users.UpdateProfile;

public static class UpdateProfileHandler
{
    public static async Task<Result> Handle(
        UpdateProfileCommand command,
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
        {
            return Result.Failure(Error.Unauthorized(
                "User.NotAuthenticated",
                "Acesso negado. Token ausente ou inválido."));
        }

        var user = await userRepository.GetByIdAsync(currentUserService.UserId, cancellationToken);

        if (user is null)
            return Result.Failure(Error.NotFound(
                "User.NotFound",
                "Usuário não encontrado no sistema."));

        var isNicknameTaken = await userRepository.IsNicknameTakenAsync(
            command.Nickname,
            user.Id,
            cancellationToken);

        if (isNicknameTaken)
        {
            return Result.Failure(Error.Conflict(
                "User.NicknameTaken",
                "Este nickname já está em uso por outra pessoa."));
        }

        var updateResult = user.SetNickname(command.Nickname);

        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        await userRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}
