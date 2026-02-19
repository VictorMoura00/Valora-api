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
                title: "Bad Request",
                detail: result.Error.Description,
                type: "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                extensions: new Dictionary<string, object?> { { "errors", new[] { result.Error } } }
            ),
            
            ErrorType.NotFound => Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Not Found",
                detail: result.Error.Description,
                type: "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            ),
            
            ErrorType.Conflict => Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Conflict",
                detail: result.Error.Description,
                type: "https://tools.ietf.org/html/rfc7231#section-6.5.8"
            ),
            
            _ => Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error",
                detail: "An unexpected error occurred.",
                type: "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            )
        };
    }
}