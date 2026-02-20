using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;
using Valora.Application.UseCases.Categories.Update;
using Valora.Domain.Common.Interfaces;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;

namespace Valora.UnitTests.Application.UseCases.Categories;

public class UpdateCategoryHandlerTests
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryHandlerTests()
    {
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
    }

    [Fact]
    public async Task Handle_DeveRetornarFalha_QuandoCategoriaNaoExistir()
    {
        // Arrange
        var command = new UpdateCategoryCommand(Guid.NewGuid(), "Novo Nome", "Nova Descrição");

        _categoryRepository.GetByIdAsync(command.Id).Returns((Category?)null);

        // Act
        var result = await UpdateCategoryHandler.Handle(
            command,
            _categoryRepository,
            _unitOfWork,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Category.NotFound", result.Error.Code);

        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DeveRetornarFalha_QuandoNomeJaEstiverEmUsoPorOutraCategoria()
    {
        // Arrange
        var command = new UpdateCategoryCommand(Guid.NewGuid(), "Nome Existente", "Nova Descrição");
        var categoriaAtual = new Category("Nome Antigo", "Descrição Antiga");
        var categoriaConflitante = new Category("Nome Existente", "Outra Descrição");

        _categoryRepository.GetByIdAsync(command.Id).Returns(categoriaAtual);
        _categoryRepository.GetByNameAsync(command.Name).Returns(categoriaConflitante);

        // Act
        var result = await UpdateCategoryHandler.Handle(
            command,
            _categoryRepository,
            _unitOfWork,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Category.DuplicateName", result.Error.Code);

        await _categoryRepository.DidNotReceive().UpdateAsync(Arg.Any<Category>());
        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DeveAtualizarESalvar_QuandoDadosForemValidos()
    {
        // Arrange
        var command = new UpdateCategoryCommand(Guid.NewGuid(), "Nome Atualizado", "Descrição Atualizada");
        var categoria = new Category("Nome Antigo", "Descrição Antiga");

        _categoryRepository.GetByIdAsync(command.Id).Returns(categoria);
        _categoryRepository.GetByNameAsync(command.Name).Returns((Category?)null);

        // Act
        var result = await UpdateCategoryHandler.Handle(
            command,
            _categoryRepository,
            _unitOfWork,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Nome Atualizado", categoria.Name);
        Assert.Equal("Descrição Atualizada", categoria.Description);

        await _categoryRepository.Received(1).UpdateAsync(categoria);
        await _unitOfWork.Received(1).CommitAsync(CancellationToken.None);
    }
}