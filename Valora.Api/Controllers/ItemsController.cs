using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Valora.Api.Controllers.Abstractions;
using Valora.Application.UseCases.Items.Create;
using Valora.Application.UseCases.Items.GetById;
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
}
