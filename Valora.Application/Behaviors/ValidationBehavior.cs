using FluentValidation;
using MediatR;
using Valora.Domain.Common.Results;

namespace Valora.Applicantion.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .ToList();

        // 3. Se não tem falhas, segue para o Handler
        if (!failures.Any())
        {
            return await next();
        }

        // 4. Se tem falhas, cria um Erro de Validação
        // Formata: "Campo: Erro; OutroCampo: OutroErro"
        var errors = string.Join("; ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}").Distinct());
        
        var validationError = Error.Validation("Validation.Error", errors);

        // 5. Mágica via Reflection para criar o Result<T> ou Result de Falha
        // (Isso é necessário porque TResponse é genérico)
        var responseType = typeof(TResponse);

        // Se for Result genérico (Result<T>)
        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var resultType = responseType.GetGenericArguments()[0];
            var failureMethod = typeof(Result<>)
                .MakeGenericType(resultType)
                .GetMethod(nameof(Result<object>.Failure)); // Pega o método estático Failure

            return (TResponse)failureMethod!.Invoke(null, new object[] { validationError })!;
        }

        // Se for Result simples (Result void)
        return (TResponse)Result.Failure(validationError);
    }
}