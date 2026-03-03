using FluentAssertions;
using NSubstitute;
using Valora.Application.Common.Interfaces;
using Valora.Application.UseCases.Users.UpdateProfile;
using Valora.Domain.Common.Interfaces;
using Valora.Domain.Common.Results;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;

namespace Valora.UnitTests.Application.UseCases.Users.UpdateProfile;

public class UpdateProfileHandlerTests
{
    private readonly ICurrentUserService _currentUserServiceMock = Substitute.For<ICurrentUserService>();
    private readonly IUserRepository _userRepositoryMock = Substitute.For<IUserRepository>();
    private readonly IUnitOfWork _unitOfWorkMock = Substitute.For<IUnitOfWork>();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    [Fact(DisplayName = "Handle deve retornar falha Unauthorized quando usuário não estiver autenticado")]
    public async Task Handle_Should_ReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var command = new UpdateProfileCommand("NewNickname");
        _currentUserServiceMock.IsAuthenticated.Returns(false);

        // Act
        var result = await UpdateProfileHandler.Handle(
            command,
            _currentUserServiceMock,
            _userRepositoryMock,
            _unitOfWorkMock,
            _cancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
        result.Error.Code.Should().Be("User.NotAuthenticated");
    }

    [Fact(DisplayName = "Handle deve retornar falha NotFound quando usuário não existir no banco")]
    public async Task Handle_Should_ReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var command = new UpdateProfileCommand("NewNickname");
        var userId = Guid.NewGuid();

        _currentUserServiceMock.IsAuthenticated.Returns(true);
        _currentUserServiceMock.UserId.Returns(userId);
        _userRepositoryMock.GetByIdAsync(userId, _cancellationToken).Returns((User?)null);

        // Act
        var result = await UpdateProfileHandler.Handle(
            command,
            _currentUserServiceMock,
            _userRepositoryMock,
            _unitOfWorkMock,
            _cancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        result.Error.Code.Should().Be("User.NotFound");
    }

    [Fact(DisplayName = "Handle deve retornar falha Conflict quando nickname já estiver em uso")]
    public async Task Handle_Should_ReturnConflict_WhenNicknameIsTaken()
    {
        // Arrange
        var command = new UpdateProfileCommand("TakenNickname");
        var userId = Guid.NewGuid();
        var user = new User(userId, "test@test.com");

        _currentUserServiceMock.IsAuthenticated.Returns(true);
        _currentUserServiceMock.UserId.Returns(userId);
        _userRepositoryMock.GetByIdAsync(userId, _cancellationToken).Returns(user);

        _userRepositoryMock.IsNicknameTakenAsync(command.Nickname, userId, _cancellationToken).Returns(true);

        // Act
        var result = await UpdateProfileHandler.Handle(
            command,
            _currentUserServiceMock,
            _userRepositoryMock,
            _unitOfWorkMock,
            _cancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
        result.Error.Code.Should().Be("User.NicknameTaken");
    }

    [Fact(DisplayName = "Handle deve retornar falha Validation quando nickname for inválido pelas regras da entidade")]
    public async Task Handle_Should_ReturnValidationFailure_WhenNicknameIsInvalid()
    {
        // Arrange
        var command = new UpdateProfileCommand("");
        var userId = Guid.NewGuid();
        var user = new User(userId, "test@test.com");

        _currentUserServiceMock.IsAuthenticated.Returns(true);
        _currentUserServiceMock.UserId.Returns(userId);
        _userRepositoryMock.GetByIdAsync(userId, _cancellationToken).Returns(user);

        _userRepositoryMock.IsNicknameTakenAsync(command.Nickname, userId, _cancellationToken).Returns(false);

        // Act
        var result = await UpdateProfileHandler.Handle(
            command,
            _currentUserServiceMock,
            _userRepositoryMock,
            _unitOfWorkMock,
            _cancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
        result.Error.Code.Should().Be("User.EmptyNickname");
    }

    [Fact(DisplayName = "Handle deve atualizar perfil e comitar quando dados forem válidos")]
    public async Task Handle_Should_UpdateProfileAndCommit_WhenDataIsValid()
    {
        // Arrange
        var command = new UpdateProfileCommand("ValidNickname");
        var userId = Guid.NewGuid();
        var user = new User(userId, "test@test.com");

        _currentUserServiceMock.IsAuthenticated.Returns(true);
        _currentUserServiceMock.UserId.Returns(userId);
        _userRepositoryMock.GetByIdAsync(userId, _cancellationToken).Returns(user);

        _userRepositoryMock.IsNicknameTakenAsync(command.Nickname, userId, _cancellationToken).Returns(false);

        // Act
        var result = await UpdateProfileHandler.Handle(
            command,
            _currentUserServiceMock,
            _userRepositoryMock,
            _unitOfWorkMock,
            _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Nickname.Should().Be(command.Nickname);

        await _userRepositoryMock.Received(1).UpdateAsync(user, _cancellationToken);
        await _unitOfWorkMock.Received(1).CommitAsync(_cancellationToken);
    }
}
