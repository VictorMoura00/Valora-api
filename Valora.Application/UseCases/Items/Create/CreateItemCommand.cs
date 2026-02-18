using MediatR;
using Valora.Domain.Common.Results;

namespace Valora.Application.UseCases.Items.Create;

// IRequest<Result<Guid>> diz: "Eu sou um pedido que, quando processado, devolve um ID ou Erro"
public record CreateItemCommand(
    string Name, 
    string Description, 
    string Category, 
    decimal Price
) : IRequest<Result<Guid>>;