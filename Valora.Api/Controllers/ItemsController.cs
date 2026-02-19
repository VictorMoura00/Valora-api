using MediatR;
using Microsoft.AspNetCore.Mvc;
using Valora.Api.Controllers.Abstractions;
using Valora.Application.UseCases.Items.Create;
using Valora.Application.UseCases.Items.GetById;
using Valora.Application.UseCases.Items.Listing;
using Valora.Domain.Common.Results;

namespace Valora.Api.Controllers;

[Route("api/items")]
public class ItemsController(ISender _sender) : ApiController
{
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateItemCommand command)
    {
        var result = await _sender.Send(command);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return CreatedAtAction(
            nameof(Create), 
            new { id = result.Value }, 
            result.Value
        );
    }
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ItemResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _sender.Send(new GetItemByIdQuery(id));
        return result.IsFailure ? HandleFailure(result) : Ok(result.Value);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<ItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] Guid? categoryId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        var query = new ListItemsQuery(categoryId, page, pageSize);
        var result = await _sender.Send(query);
        return result.IsFailure ? HandleFailure(result) : Ok(result.Value);
    }
}