using Microsoft.AspNetCore.Mvc;
using Valora.Domain.Common.Results;

namespace Valora.Api.Controllers.Abstractions;

[ApiController]
public abstract class ApiController : ControllerBase
{
    protected IActionResult HandleFailure(Result result)
    {
        return result.Error.Type switch
        {
            ErrorType.Validation => Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: result.Error.Code,
                detail: result.Error.Description
            ),
            
            ErrorType.NotFound => Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: result.Error.Code,
                detail: result.Error.Description
            ),
            
            ErrorType.Conflict => Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: result.Error.Code,
                detail: result.Error.Description
            ),
            
            _ => Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Server.InternalError",
                detail: result.Error.Description
            )
        };
    }
}