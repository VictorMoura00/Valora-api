using MediatR;
using Valora.Application.UseCases.Categories.GetById;
using Valora.Domain.Common.Results;
using Valora.Domain.Repositories;

namespace Valora.Application.UseCases.Categories.List;

public class ListCategoriesHandler(ICategoryRepository _categoryRepository) 
    : IRequestHandler<ListCategoriesQuery, Result<PaginatedList<CategoryResponse>>>
{
    public async Task<Result<PaginatedList<CategoryResponse>>> Handle(ListCategoriesQuery request, CancellationToken cancellationToken)
    {
        // Usa o método genérico que criamos no BaseRepository
        var paginatedCategories = await _categoryRepository.GetPaginatedAsync(
            request.Page, 
            request.PageSize, 
            cancellationToken
        );

        // Mapeia de Entidade -> Response DTO
        var responseItems = paginatedCategories.Items.Select(c => new CategoryResponse(
            c.Id,
            c.Name,
            c.Description,
            c.Schema.Select(f => new CategoryFieldResponse(f.Name, f.Type.ToString(), f.IsRequired)).ToList()
        )).ToList();

        // Cria uma nova lista paginada com o tipo correto
        var response = new PaginatedList<CategoryResponse>(
            responseItems,
            paginatedCategories.TotalCount,
            paginatedCategories.PageNumber, 
            request.PageSize // Usamos o pageSize do request para garantir consistência
        );

        return response;
    }
}