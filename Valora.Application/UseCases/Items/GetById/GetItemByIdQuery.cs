using MediatR;
using Valora.Domain.Common.Results;

namespace Valora.Application.UseCases.Items.GetById;

public record GetItemByIdQuery(Guid Id) : IRequest<Result<ItemResponse>>;