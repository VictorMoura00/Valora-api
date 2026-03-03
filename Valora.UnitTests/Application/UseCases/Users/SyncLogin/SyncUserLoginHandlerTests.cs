using FluentAssertions;
using NSubstitute;
using Valora.Application.Common.Interfaces;
using Valora.Application.UseCases.Users.SyncLogin;
using Valora.Domain.Common.Interfaces;
using Valora.Domain.Common.Results;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;

namespace Valora.UnitTests.Application.UseCases.Users.SyncLogin;

public class SyncUserLoginHandlerTests
{
    private readonly ICurrentUserService _currentUserServiceMock = Substitute.For<ICurrentUserService>();
    private readonly IUserRepository _userRepositoryMock = Substitute.For<IUserRepository>();
    private readonly IUnitOfWork _unitOfWorkMock = Substitute.For<IUnitOfWork>();
    private readonly SyncUserLoginCommand _command = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    [Fact(DisplayName = "Handle deve retornar falha Unauthorized quando usuário não estiver autenticado")]
    public async Task Handle_Should_ReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _currentUserServiceMock.IsAuthenticated.Returns(false);

        // Act
        var result = await SyncUserLoginHandler.Handle(
            _command,
            _currentUserServiceMock,
            _userRepositoryMock,
            _unitOfWorkMock,
            _cancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Unauthorized);
        result.Error.Code.Should().Be("User.NotAuthenticated");
    }

    [Fact(DisplayName = "Handle deve retornar falha Validation quando email estiver ausente no token")]
    public async Task Handle_Should_ReturnValidationFailure_WhenEmailIsMissing()
    {
        // Arrange
        _currentUserServiceMock.IsAuthenticated.Returns(true);
        _currentUserServiceMock.UserId.Returns(Guid.NewGuid());
        _currentUserServiceMock.Email.Returns(string.Empty);

        // Act
        var result = await SyncUserLoginHandler.Handle(
            _command,
            _currentUserServiceMock,
            _userRepositoryMock,
            _unitOfWorkMock,
            _cancellationToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
        result.Error.Code.Should().Be("User.MissingEmail");
    }

    [Fact(DisplayName = "Handle deve criar novo usuário e comitar quando for o primeiro acesso")]
    public async Task Handle_Should_CreateUserAndCommit_WhenFirstAccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@test.com";

        _currentUserServiceMock.IsAuthenticated.Returns(true);
        _currentUserServiceMock.UserId.Returns(userId);
        _currentUserServiceMock.Email.Returns(email);

        _userRepositoryMock.GetByIdAsync(userId, _cancellationToken).Returns((User?)null);

        // Act
        var result = await SyncUserLoginHandler.Handle(
            _command,
            _currentUserServiceMock,
            _userRepositoryMock,
            _unitOfWorkMock,
            _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _userRepositoryMock.Received(1).AddAsync(Arg.Is<User>(u => u.Id == userId && u.Email == email), _cancellationToken);
        await _userRepositoryMock.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await _unitOfWorkMock.Received(1).CommitAsync(_cancellationToken);
    }

    [Fact(DisplayName = "Handle deve atualizar LastLoginAt e comitar quando usuário já existir")]
    public async Task Handle_Should_UpdateLastLoginAtAndCommit_WhenUserAlreadyExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@test.com";
        var existingUser = new User(userId, email);

        _currentUserServiceMock.IsAuthenticated.Returns(true);
        _currentUserServiceMock.UserId.Returns(userId);
        _currentUserServiceMock.Email.Returns(email);

        _userRepositoryMock.GetByIdAsync(userId, _cancellationToken).Returns(existingUser);

        // Act
        var result = await SyncUserLoginHandler.Handle(
            _command,
            _currentUserServiceMock,
            _userRepositoryMock,
            _unitOfWorkMock,
            _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _userRepositoryMock.DidNotReceive().AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await _userRepositoryMock.Received(1).UpdateAsync(existingUser, _cancellationToken);
        await _unitOfWorkMock.Received(1).CommitAsync(_cancellationToken);
    }
}
