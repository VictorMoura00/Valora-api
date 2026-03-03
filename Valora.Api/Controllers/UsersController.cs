using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Valora.Api.Controllers.Abstractions;
using Valora.Application.UseCases.Users.SyncLogin;
using Valora.Application.UseCases.Users.UpdateProfile;
using Valora.Domain.Common.Results;
using Wolverine;

namespace Valora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IMessageBus bus) : ApiController
{
    [HttpPost("sync")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SyncLogin(CancellationToken cancellationToken)
    {
        var command = new SyncUserLoginCommand();

        var result = await bus.InvokeAsync<Result>(command, cancellationToken);

        return result.IsFailure ? HandleFailure(result) : Ok();
    }

    [HttpPut("me/profile")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateProfileCommand command,
        CancellationToken cancellationToken)
    {
        var result = await bus.InvokeAsync<Result>(command, cancellationToken);

        return result.IsFailure ? HandleFailure(result) : NoContent();
    }
}
