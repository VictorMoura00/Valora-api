using MediatR;
using Valora.Domain.Common.Results;

namespace Valora.Application.UseCases.Items.Create;

public record CreateItemCommand(
    string Name, 
    string Description, 
    string Category
) : IRequest<Result<Guid>>;