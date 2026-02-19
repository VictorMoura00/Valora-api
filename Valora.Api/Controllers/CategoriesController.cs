using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Valora.Api.Controllers.Abstractions;
using Valora.Application.UseCases.Categories.Create;
using Valora.Application.UseCases.Categories.GetById;
using Valora.Domain.Common.Results;
using Wolverine; // Adicione este using

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
        // O Wolverine usa o InvokeAsync para aguardar uma resposta
        var result = await _bus.InvokeAsync<Result<Guid>>(command);

        if (result.IsFailure)
            return HandleFailure(result);

        return Ok(result.Value);
        //return CreatedAtAction(
        //    //nameof(GetById),
        //    new { id = result.Value },
        //    result.Value
        //);
    }

    //[HttpGet("{id:guid}")]
    //[ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> GetById(Guid id)
    //{
    //    var query = new GetCategoryByIdQuery(id);
    //    var result = await _sender.Send(query);

    //    return result.IsFailure ? HandleFailure(result) : Ok(result.Value);
    //}
    
    //[HttpGet]
    //[ProducesResponseType(typeof(PaginatedList<CategoryResponse>), StatusCodes.Status200OK)]
    //public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    //{
    //    var query = new ListCategoriesQuery(page, pageSize);
    //    var result = await _sender.Send(query);

    //    if (result.IsFailure)
    //    {
    //        return HandleFailure(result);
    //    }

    //    return Ok(result.Value);
    //}
}