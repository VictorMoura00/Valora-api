using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Valora.Api.Controllers.Abstractions;
using Valora.Application.UseCases.Categories.Create;
using Valora.Application.UseCases.Categories.GetById;
using Valora.Application.UseCases.Categories.List;
using Valora.Application.UseCases.Categories.Update;
using Valora.Domain.Common.Pagination;
using Valora.Domain.Common.Results;
using Wolverine;

namespace Valora.Api.Controllers;

[Route("api/categories")]
public class CategoriesController(IMessageBus _bus) : ApiController
{
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
    {
        var result = await _bus.InvokeAsync<Result<Guid>>(command);

        if (result.IsFailure)
            return HandleFailure(result);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value },
            result.Value
        );
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetCategoryByIdQuery(id);
        
        var result = await _bus.InvokeAsync<Result<CategoryResponse>>(query);

        return result.IsFailure ? HandleFailure(result) : Ok(result.Value);
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<CategoryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new ListCategoriesQuery(page, pageSize);
        
        var result = await _bus.InvokeAsync<Result<PaginatedList<CategoryResponse>>>(query);

        return result.IsFailure ? HandleFailure(result) : Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand command)
    {
        if (id != command.Id)
            return BadRequest(new ProblemDetails
            {
                Title = "Inconsistência de Dados",
                Detail = "O ID da rota não coincide com o ID do corpo da requisição."
            });

        var result = await _bus.InvokeAsync<Result>(command);

        if (result.IsFailure)
            return HandleFailure(result);

        return NoContent();
    }
}