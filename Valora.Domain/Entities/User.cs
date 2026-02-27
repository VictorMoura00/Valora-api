using System;
using Valora.Domain.Common.Abstractions;
using Valora.Domain.Common.Results;

namespace Valora.Domain.Entities;

public class User : Entity, IAggregateRoot
{
    public string Email { get; private set; }
    public string? Nickname { get; private set; }
    public DateTimeOffset LastLoginAt { get; private set; }
    public bool IsBlocked { get; private set; }

    /// <summary>
    /// Construtor restrito. O ID não é gerado pelo banco, mas sim injetado 
    /// a partir do Identity Provider (Claim 'sub' do JWT).
    /// </summary>
    public User(Guid id, string email)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("O ID do usuário é obrigatório e deve vir do token JWT.", nameof(id));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("O e-mail é obrigatório.", nameof(email));

        Id = id;
        Email = email.ToLowerInvariant().Trim();
        LastLoginAt = DateTimeOffset.UtcNow;
        IsBlocked = false;
    }

    public Result SetNickname(string nickname)
    {
        if (string.IsNullOrWhiteSpace(nickname))
            return Result.Failure(Error.Validation(
                "User.EmptyNickname",
                "O nickname não pode ser vazio."));

        var trimmedNickname = nickname.Trim();
        if (trimmedNickname.Length is < 3 or > 30)
            return Result.Failure(Error.Validation(
                "User.InvalidNicknameLength",
                "O nickname deve ter entre 3 e 30 caracteres."));

        Nickname = trimmedNickname;
        SetUpdated();

        return Result.Success();
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTimeOffset.UtcNow;
        SetUpdated();
    }

    public void Block()
    {
        IsBlocked = true;
        SetUpdated();
    }

    public void Unblock()
    {
        IsBlocked = false;
        SetUpdated();
    }
}
