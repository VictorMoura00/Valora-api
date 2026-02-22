using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Valora.Api.Controllers.Abstractions;
using Valora.Application.UseCases.Items.Create;
using Valora.Application.UseCases.Items.Delete;
using Valora.Application.UseCases.Items.GetById;
using Valora.Application.UseCases.Items.ListByCategory;
using Valora.Application.UseCases.Items.Update;
using Valora.Domain.Common.Pagination;
using Valora.Domain.Common.Results;
using Wolverine;

namespace Valora.Api.Controllers;

[Route("api/items")]
public class ItemsController : ApiController
{
    private readonly IMessageBus _bus;

    public ItemsController(IMessageBus bus)
    {
        _bus = bus;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateItemCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _bus.InvokeAsync<Result<Guid>>(command, cancellationToken);

        return result.IsFailure ? HandleFailure(result)
            : CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetItemByIdQuery(id);

        var result = await _bus.InvokeAsync<Result<ItemDto>>(query, cancellationToken);

        return result.IsFailure ? HandleFailure(result)
            : Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateItemCommand command,
        CancellationToken cancellationToken)
    {
        var commandWithId = command with { Id = id };
        var result = await _bus.InvokeAsync<Result<Guid>>(command, cancellationToken);

        return result.IsFailure ? HandleFailure(result)
            : NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteItemCommand(id);

        var result = await _bus.InvokeAsync<Result>(command, cancellationToken);

        return result.IsFailure ? HandleFailure(result) : NoContent();
    }

    [HttpGet("~/api/categories/{categoryId:guid}/items")]
    [ProducesResponseType(typeof(PaginatedList<ItemSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListByCategory(
        [FromRoute] Guid categoryId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new ListItemsByCategoryQuery(categoryId, pageNumber, pageSize);
        var result = await _bus.InvokeAsync<Result<PaginatedList<ItemSummaryDto>>>(query, cancellationToken);

        return result.IsFailure ? HandleFailure(result) : Ok(result.Value);
    }
}
