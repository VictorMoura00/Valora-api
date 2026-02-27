using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Valora.Api.Controllers.Abstractions;
using Valora.Application.UseCases.User.SyncLogin;
using Valora.Application.UseCases.Users.SyncLogin;
using Valora.Domain.Common.Results;
using Wolverine;

namespace Valora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ApiController
{
    private readonly IMessageBus _bus;

    public UsersController(IMessageBus bus)
    {
        _bus = bus;
    }

    /// <summary>
    /// Sincroniza o login do Identity Provider com o banco de dados local.
    /// Deve ser chamado pelo Front-End logo após o usuário realizar o login com sucesso.
    /// </summary>
    [HttpPost("sync")]
    [Authorize] // Garante que apenas requisições com JWT válido do Auth0 entrem aqui
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SyncLogin(CancellationToken cancellationToken)
    {
        var command = new SyncUserLoginCommand();

        var result = await _bus.InvokeAsync<Result>(command, cancellationToken);

        if (result.IsFailure)
        {
            // Substitua por HandleFailure(result) se você usar uma BaseController
            return HandleFailure(result);
        }

        return Ok();
    }
}
