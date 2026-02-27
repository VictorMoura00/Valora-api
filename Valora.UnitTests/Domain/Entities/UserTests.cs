using FluentAssertions;
using Valora.Domain.Entities;

namespace Valora.UnitTests.Domain.Entities;

public class UserTests
{
    private readonly Guid _validId = Guid.NewGuid();
    private const string ValidEmail = "test@example.com";

    [Fact(DisplayName = "Construtor deve instanciar User corretamente com dados válidos")]
    public void Constructor_Should_CreateUser_WhenValidData()
    {
        // Arrange

        // Act
        var user = new User(_validId, ValidEmail);

        // Assert
        user.Id.Should().Be(_validId);
        user.Email.Should().Be(ValidEmail);
        user.Nickname.Should().BeNull();
        user.IsBlocked.Should().BeFalse();
        user.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        user.LastLoginAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact(DisplayName = "Construtor deve lançar exceção se Id for vazio")]
    public void Constructor_Should_ThrowException_WhenIdIsEmpty()
    {
        // Arrange

        // Act
        Action action = () => new User(Guid.Empty, ValidEmail);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Theory(DisplayName = "Construtor deve lançar exceção se Email for inválido")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_Should_ThrowException_WhenEmailIsInvalid(string invalidEmail)
    {
        // Arrange

        // Act
        Action action = () => new User(_validId, invalidEmail);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Theory(DisplayName = "SetNickname deve retornar sucesso quando válido")]
    [InlineData("Dev")]
    [InlineData("John Doe")]
    [InlineData("VeryLongNicknameButValid123456")]
    public void SetNickname_Should_ReturnSuccess_WhenValid(string nickname)
    {
        // Arrange
        var user = new User(_validId, ValidEmail);

        // Act
        var result = user.SetNickname(nickname);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Nickname.Should().Be(nickname.Trim());
        user.UpdatedAt.Should().NotBeNull();
    }

    [Theory(DisplayName = "SetNickname deve retornar falha quando inválido")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("ab")]
    [InlineData("ThisNicknameIsWayTooLongAndShouldFailTheValidation")]
    public void SetNickname_Should_ReturnFailure_WhenInvalid(string invalidNickname)
    {
        // Arrange
        var user = new User(_validId, ValidEmail);

        // Act
        var result = user.SetNickname(invalidNickname);

        // Assert
        result.IsFailure.Should().BeTrue();
        user.Nickname.Should().BeNull();
    }

    [Fact(DisplayName = "RecordLogin deve atualizar LastLoginAt")]
    public void RecordLogin_Should_UpdateLastLoginAt()
    {
        // Arrange
        var user = new User(_validId, ValidEmail);
        var initialLogin = user.LastLoginAt;

        Thread.Sleep(10);

        // Act
        user.RecordLogin();

        // Assert
        user.LastLoginAt.Should().BeAfter(initialLogin);
        user.UpdatedAt.Should().NotBeNull();
    }

    [Fact(DisplayName = "Block e Unblock devem alternar a propriedade IsBlocked corretamente")]
    public void BlockAndUnblock_Should_ToggleIsBlocked()
    {
        // Arrange
        var user = new User(_validId, ValidEmail);

        // Act 1
        user.Block();

        // Assert 1
        user.IsBlocked.Should().BeTrue();
        user.UpdatedAt.Should().NotBeNull();

        // Act 2
        user.Unblock();

        // Assert 2
        user.IsBlocked.Should().BeFalse();
    }
}
