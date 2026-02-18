using MediatR;
using Valora.Application.UseCases.Items.GetById;
using Valora.Domain.Common.Results;
using Valora.Domain.Repositories;

namespace Valora.Application.UseCases.Items.GetById;

public class GetItemByIdHandler(IItemRepository _repository) 
    : IRequestHandler<GetItemByIdQuery, Result<ItemResponse>>
{
    public async Task<Result<ItemResponse>> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdAsync(request.Id);

        if (item is null)
            return Error.NotFound("Item.NotFound", $"O item com o Id '{request.Id}' não foi encontrado.");

        // 3. Mapeia Entidade -> DTO (Resposta)
        var response = new ItemResponse(
            item.Id,
            item.Name,
            item.Description,
            item.Category,
            item.AverageRating
        );

        return response; // Conversão implícita para Result<ItemResponse>
    }
}