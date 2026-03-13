using System.Threading;
using System.Threading.Tasks;
using Valora.Application.Common.Interfaces;
using Valora.Domain.Common.Interfaces;
using Valora.Domain.Common.Results;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;

namespace Valora.Application.UseCases.Users.SyncLogin;

public static class SyncUserLoginHandler
{
    public static async Task<Result> Handle(
        SyncUserLoginCommand command,
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

        var userId = currentUserService.UserId;
        var email = currentUserService.Email;

        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Failure(Error.Validation(
                "User.MissingEmail",
                "O token JWT não contém a claim de e-mail obrigatória."));
        }

        var user = await userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            user = new User(userId, email);
            await userRepository.AddAsync(user, cancellationToken);
        }
        else
        {
            user.RecordLogin();
            await userRepository.UpdateAsync(user, cancellationToken);
        }

        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}
