namespace Valora.Domain.Common.Results;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    protected Result(bool isSuccess, Error error)
    {
        // Validação defensiva
        if (isSuccess && error != Error.None)
        {
            throw new InvalidOperationException("Um resultado de sucesso não pode conter um erro.");
        }

        if (!isSuccess && error == Error.None)
        {
            throw new InvalidOperationException("Um resultado de falha deve conter um erro.");
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    // Fábricas para resultados sem valor (Void)
    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
    
    // Se quiser criar um genérico a partir daqui, precisamos instanciar explicitamente a classe filha
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);
    public static Result<T> Failure<T>(Error error) => Result<T>.Failure(error);
}