using MediatR;
using Valora.Domain.Common.Results;

namespace Valora.Application.UseCases.Items.GetById;

// "Eu quero um ItemResponse baseado nesse Id"
public record GetItemByIdQuery(Guid Id) : IRequest<Result<ItemResponse>>;