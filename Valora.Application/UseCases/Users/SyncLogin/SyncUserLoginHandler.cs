using System.Threading;
using System.Threading.Tasks;
using Valora.Application.Common.Interfaces;
using Valora.Domain.Common.Interfaces;
using Valora.Domain.Common.Results;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;

namespace Valora.Application.UseCases.Users.SyncLogin
{
    public static class SyncUserLoginHandler
    {
        public static async Task<Result> Handle(
            SyncUserLoginCommand command,
            ICurrentUserService currentUserService,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            CancellationToken cancellationToken)
        {
            // 1. Validação de segurança básica: O token é válido?
            if (!currentUserService.IsAuthenticated)
            {
                return Result.Failure(Error.Unauthorized(
                    "User.NotAuthenticated",
                    "Acesso negado. Token ausente ou inválido."));
            }

            var userId = currentUserService.UserId;
            var email = currentUserService.Email;

            // É raro, mas alguns provedores de identidade podem não enviar o email no token
            if (string.IsNullOrWhiteSpace(email))
            {
                return Result.Failure(Error.Validation(
                    "User.MissingEmail",
                    "O token JWT não contém a claim de e-mail obrigatória."));
            }

            // 2. Busca o usuário no banco de dados do Valora
            var user = await userRepository.GetByIdAsync(userId, cancellationToken);

            if (user is null)
            {
                // 3A. Primeiro Acesso: Cria o registro do usuário
                user = new Domain.Entities.User(userId, email);
                await userRepository.AddAsync(user, cancellationToken);
            }
            else
            {
                // 3B. Acessos Seguintes: Apenas atualiza a métrica de acesso
                user.RecordLogin();
                await userRepository.UpdateAsync(user, cancellationToken);
            }

            // 4. Salva as alterações (Insert ou Update)
            await unitOfWork.CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
