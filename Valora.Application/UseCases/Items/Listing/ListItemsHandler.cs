//using MediatR;
//using Valora.Application.UseCases.Items.GetById;
//using Valora.Domain.Common.Results;
//using Valora.Domain.Repositories;

//namespace Valora.Application.UseCases.Items.Listing;

//public class ListItemsHandler(IItemRepository _itemRepository) 
//    : IRequestHandler<ListItemsQuery, Result<PaginatedList<ItemResponse>>>
//{
//    public async Task<Result<PaginatedList<ItemResponse>>> Handle(ListItemsQuery request, CancellationToken cancellationToken)
//    {
//        var paginatedItems = await _itemRepository.GetByCategoryAsync(
//            request.CategoryId, 
//            request.Page, 
//            request.PageSize, 
//            cancellationToken
//        );

//        // Mapeamento
//        var responseItems = paginatedItems.Items.Select(i => new ItemResponse(
//            i.Id,
//            i.CategoryId,
//            "Carregue se precisar", // Otimização: Em listas grandes, evitamos buscar o nome da categoria um por um
//            i.Data,
//            i.AverageRating
//        )).ToList();

//        return new PaginatedList<ItemResponse>(
//            responseItems,
//            paginatedItems.TotalCount,
//            paginatedItems.PageNumber,
//            request.PageSize
//        );
//    }
//}