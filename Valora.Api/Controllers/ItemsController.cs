using MediatR;
using Microsoft.AspNetCore.Mvc;
using Valora.Api.Controllers.Abstractions;
using Valora.Application.UseCases.Items.Create;
using Valora.Application.UseCases.Items.GetById;

namespace Valora.Api.Controllers;

[Route("api/items")]
public class ItemsController(IMediator _mediator) : ApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateItemCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return HandleFailure(result);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value },
            result.Value
        );
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetItemByIdQuery(id);
        
        var result = await _mediator.Send(query);

        return result.Match(
            onSuccess: response => Ok(response),
            onFailure: error => HandleFailure(result)
        );
    }
}