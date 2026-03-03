using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Valora.Application.UseCases.Categories.UpdateSchema;
using Valora.Domain.Common.Interfaces;
using Valora.Domain.Common.Results;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;

namespace Valora.UnitTests.Application.UseCases.Categories.UpdateSchema;

public class UpdateCategorySchemaHandlerTests
{
    private readonly ICategoryRepository _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
    private readonly IUnitOfWork _unitOfWorkMock = Substitute.For<IUnitOfWork>();

    [Fact(DisplayName = "Deve retornar falha (NotFound) quando a categoria não existir")]
    public async Task Handle_Should_ReturnFailure_WhenCategoryDoesNotExist()
    {
        // Arrange
        var command = new UpdateCategorySchemaCommand(Guid.NewGuid(), new List<CategoryFieldDto>());

        _categoryRepositoryMock.GetByIdAsync(command.Id).Returns((Category?)null);

        // Act
        var result = await UpdateCategorySchemaHandler.Handle(
            command,
            _categoryRepositoryMock,
            _unitOfWorkMock,
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Category.NotFound");

        await _categoryRepositoryMock.DidNotReceiveWithAnyArgs().UpdateAsync(default!);
        await _unitOfWorkMock.DidNotReceiveWithAnyArgs().CommitAsync(default);
    }

    [Fact(DisplayName = "Deve retornar falha (Conflict) quando o domínio rejeitar o novo schema (ex: campos duplicados)")]
    public async Task Handle_Should_ReturnFailure_WhenDomainRejectsSchema()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category("Hardware", "Peças de computador");

        // Criando um comando com campos duplicados para forçar a falha na entidade
        var invalidSchemaCommand = new UpdateCategorySchemaCommand(categoryId, new List<CategoryFieldDto>
        {
            new("Potência", FieldType.Number, true),
            new("Potência", FieldType.Text, false) // Nome duplicado
        });

        _categoryRepositoryMock.GetByIdAsync(invalidSchemaCommand.Id).Returns(category);

        // Act
        var result = await UpdateCategorySchemaHandler.Handle(
            invalidSchemaCommand,
            _categoryRepositoryMock,
            _unitOfWorkMock,
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Category.DuplicateField");

        await _categoryRepositoryMock.DidNotReceiveWithAnyArgs().UpdateAsync(default!);
        await _unitOfWorkMock.DidNotReceiveWithAnyArgs().CommitAsync(default);
    }

    [Fact(DisplayName = "Deve atualizar o schema e retornar sucesso quando os dados forem válidos")]
    public async Task Handle_Should_UpdateSchemaAndReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category("Veículos", "Carros e Motos");

        // O schema original para provar que ele foi substituído
        category.AddField("Placa", FieldType.Text, true);

        var validSchemaCommand = new UpdateCategorySchemaCommand(categoryId, new List<CategoryFieldDto>
        {
            new("Quilometragem", FieldType.Number, true),
            new("Cor", FieldType.Text, false)
        });

        _categoryRepositoryMock.GetByIdAsync(validSchemaCommand.Id).Returns(category);

        // Act
        var result = await UpdateCategorySchemaHandler.Handle(
            validSchemaCommand,
            _categoryRepositoryMock,
            _unitOfWorkMock,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        // Verifica se a entidade em memória foi alterada corretamente
        category.Schema.Should().HaveCount(2);
        category.Schema.Should().Contain(f => f.Name == "Quilometragem");
        category.Schema.Should().Contain(f => f.Name == "Cor");
        category.Schema.Should().NotContain(f => f.Name == "Placa"); // O campo antigo deve sumir

        await _categoryRepositoryMock.Received(1).UpdateAsync(category);
        await _unitOfWorkMock.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
}