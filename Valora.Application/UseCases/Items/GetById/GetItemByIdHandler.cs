//using MediatR;
//using Valora.Domain.Common.Results;
//using Valora.Domain.Repositories;

//namespace Valora.Application.UseCases.Items.GetById;

//public class GetItemByIdHandler(
//    IItemRepository _itemRepository, 
//    ICategoryRepository _categoryRepository
//) : IRequestHandler<GetItemByIdQuery, Result<ItemResponse>>
//{
//    public async Task<Result<ItemResponse>> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
//    {
//        var item = await _itemRepository.GetByIdAsync(request.Id);

//        if (item is null)
//        {
//            return Result.Failure<ItemResponse>(Error.NotFound("Item.NotFound", "Item não encontrado."));
//        }

//        var category = await _categoryRepository.GetByIdAsync(item.CategoryId);
//        var categoryName = category?.Name ?? "Desconhecida";

//        return new ItemResponse(
//            item.Id,
//            item.CategoryId,
//            categoryName,
//            item.Data,
//            item.AverageRating
//        );
//    }
//}