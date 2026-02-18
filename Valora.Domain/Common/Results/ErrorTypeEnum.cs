namespace Valora.Domain.Common.Results;

/// <summary>
/// Define a categoria do erro ocorrido.
/// Utilizado para mapear falhas de domínio para Status Codes HTTP na camada de API.
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// Erro genérico ou não tratado. Geralmente mapeado para HTTP 500 (Internal Server Error).
    /// </summary>
    Failure = 0,

    /// <summary>
    /// Erro de validação de dados de entrada (ex: campos nulos, formato inválido).
    /// Geralmente mapeado para HTTP 400 (Bad Request).
    /// </summary>
    Validation = 1,

    /// <summary>
    /// O recurso solicitado não foi encontrado.
    /// Geralmente mapeado para HTTP 404 (Not Found).
    /// </summary>
    NotFound = 2,

    /// <summary>
    /// Violação de regra de negócio ou estado conflitante (ex: email já cadastrado).
    /// Geralmente mapeado para HTTP 409 (Conflict).
    /// </summary>
    Conflict = 3
}