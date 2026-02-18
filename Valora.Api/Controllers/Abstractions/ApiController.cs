using Microsoft.AspNetCore.Mvc;
using Valora.Domain.Common.Results;

namespace Valora.Api.Controllers.Abstractions;

/// <summary>
/// Controlador base para todas as APIs do projeto.
/// Fornece métodos utilitários para padronizar as respostas HTTP e o tratamento de erros.
/// </summary>
[ApiController]
public abstract class ApiController : ControllerBase
{
    /// <summary>
    /// Centraliza a conversão de erros de Domínio (Result Pattern) para respostas HTTP padronizadas.
    /// Utiliza a RFC 7807 (Problem Details) para estruturar o JSON de erro.
    /// </summary>
    /// <param name="result">O objeto <see cref="Result"/> que contém a falha ocorrida.</param>
    /// <returns>
    /// Um <see cref="IActionResult"/> correspondente ao tipo do erro:
    /// <list type="bullet">
    /// <item><description><b>400 Bad Request</b>: Para erros de validação (<see cref="ErrorType.Validation"/>).</description></item>
    /// <item><description><b>404 Not Found</b>: Para recursos não encontrados (<see cref="ErrorType.NotFound"/>).</description></item>
    /// <item><description><b>409 Conflict</b>: Para violações de regras de negócio (<see cref="ErrorType.Conflict"/>).</description></item>
    /// <item><description><b>500 Internal Server Error</b>: Para falhas genéricas ou não mapeadas.</description></item>
    /// </list>
    /// </returns>
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