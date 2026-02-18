namespace Valora.Domain.Common.Results;

public class Result<T> : Result
{
    private readonly T? _value;

    public T Value => IsSuccess 
        ? _value! 
        : throw new InvalidOperationException("Não é possível acessar o valor de um resultado de falha.");

    // Construtor "protected internal" para que a classe base ou factories possam chamar
    protected internal Result(T? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    // --- MÉTODOS DE FÁBRICA ---
    public static new Result<T> Success(T value) => new(value, true, Error.None);
    public static new Result<T> Failure(Error error) => new(default, false, error);

    // --- OPERADORES IMPLÍCITOS ---
    // Permite: Result<Item> result = item;
    public static implicit operator Result<T>(T? value) => 
        value is not null ? Success(value) : Failure(Error.NullValue);

    // Permite: Result<Item> result = Error.NotFound(...);
    public static implicit operator Result<T>(Error error) => Failure(error);
    
    // Obriga quem chama a tratar o sucesso e o erro
    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<Error, TResult> onFailure)
    {
        if (IsFailure)
        {
            return onFailure(Error);
        }

        return onSuccess(Value);
    }
}