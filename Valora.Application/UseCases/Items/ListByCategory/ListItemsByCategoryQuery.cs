namespace Valora.Application.UseCases.Items.ListByCategory;

public record ListItemsByCategoryQuery(
    Guid CategoryId,
    int PageNumber = 1,
    int PageSize = 10);
