using MediatR;
using Valora.Domain.Common.Interfaces;
using Valora.Domain.Common.Results;
using Valora.Domain.Entities;
using Valora.Domain.Repositories;

namespace Valora.Application.UseCases.Items.Create;

public class CreateItemHandler(IItemRepository _itemRepository, IUnitOfWork _unitOfWork) 
    : IRequestHandler<CreateItemCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var existingItem = await _itemRepository.GetByNameAsync(request.Name);
        if (existingItem is not null)
        {
            return Result.Failure<Guid>(Error.Conflict(
                "Item.DuplicateName", 
                $"Já existe um item com o nome '{request.Name}'."
            ));
        }

        var item = new Item(
            request.Name, 
            request.Description, 
            request.Category
        );

        await _itemRepository.AddAsync(item);
        await _unitOfWork.CommitAsync(cancellationToken);

        return item.Id;
    }
}