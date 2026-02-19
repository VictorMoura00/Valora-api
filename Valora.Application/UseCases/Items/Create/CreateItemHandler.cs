using System.Text.Json; // Importante: Adicione este using
using MediatR;
using Valora.Domain.Common.Exceptions;
using Valora.Domain.Common.Interfaces;
using Valora.Domain.Common.Results;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;

namespace Valora.Application.UseCases.Items.Create;

public class CreateItemHandler(
    IItemRepository _itemRepository,
    ICategoryRepository _categoryRepository,
    IUnitOfWork _unitOfWork
) : IRequestHandler<CreateItemCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        // 1. Buscar a Categoria
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
        
        if (category is null)
        {
            return Result.Failure<Guid>(Error.NotFound(
                "Category.NotFound", 
                $"A categoria '{request.CategoryId}' não existe."
            ));
        }

        // 2. HIGIENIZAÇÃO (Correção do Erro): 
        // Converte JsonElement de volta para tipos primitivos (string, int, bool)
        // para que o MongoDB consiga salvar.
        var cleanFields = new Dictionary<string, object>();
        
        foreach (var field in request.Fields)
        {
            cleanFields[field.Key] = UnwrapJsonElement(field.Value);
        }

        // 3. Criar a Entidade Item (Usando o dicionário limpo)
        var item = new Item(request.CategoryId, cleanFields);

        // 4. Validação de Domínio
        try
        {
            item.ValidateAgainstSchema(category);
        }
        catch (DomainException ex)
        {
            return Result.Failure<Guid>(Error.Validation(
                "Item.SchemaMismatch", 
                ex.Message
            ));
        }

        // 5. Persistência
        await _itemRepository.AddAsync(item);
        await _unitOfWork.CommitAsync(cancellationToken);

        return item.Id;
    }

    // Método auxiliar para converter JsonElement em tipos reais do C#
    private static object UnwrapJsonElement(object value)
    {
        if (value is JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString() ?? string.Empty,
                JsonValueKind.Number => element.TryGetInt32(out var i) ? i : element.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null!,
                _ => element.ToString() // Fallback
            };
        }

        return value;
    }
}