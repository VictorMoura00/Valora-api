//using MediatR;
//using Valora.Application.UseCases.Items.GetById;
//using Valora.Domain.Common.Results;

//namespace Valora.Application.UseCases.Items.Listing;

//public record ListItemsQuery(
//    Guid? CategoryId, // Filtro opcional
//    int Page = 1, 
//    int PageSize = 10
//) : IRequest<Result<PaginatedList<ItemResponse>>>;