namespace Valora.Application.UseCases.Users.UpdateProfile;

/// <summary>
/// Comando para atualizar o perfil do usuário logado.
/// O ID do usuário será inferido com segurança pelo ICurrentUserService.
/// </summary>
public record UpdateProfileCommand(string Nickname);
