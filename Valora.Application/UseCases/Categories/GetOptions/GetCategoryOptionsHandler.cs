using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Valora.Domain.Common.Results;
using Valora.Domain.Repositories;

namespace Valora.Application.UseCases.Categories.GetOptions;

public static class GetCategoryOptionsHandler
{
    public static async Task<Result<IEnumerable<CategoryOptionDto>>> Handle(
        GetCategoryOptionsQuery query,
        ICategoryRepository categoryRepository,
        CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetAllAsync(cancellationToken);

        var options = categories
            .Select(c => new CategoryOptionDto(c.Id, c.Name))
            .OrderBy(c => c.Name)
            .ToList();

        return Result.Success<IEnumerable<CategoryOptionDto>>(options);
    }
}