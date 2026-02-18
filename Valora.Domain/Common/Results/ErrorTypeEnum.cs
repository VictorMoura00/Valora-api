namespace Valora.Domain.Common.Results;

public enum ErrorType
{
    Failure = 0,    // Erro genérico (500)
    Validation = 1, // Erro de entrada de dados (400)
    NotFound = 2,   // Recurso não encontrado (404)
    Conflict = 3    // Regra de negócio impedindo (409)
}