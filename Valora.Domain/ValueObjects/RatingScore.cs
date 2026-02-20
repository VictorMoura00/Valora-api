using System.Collections.Generic;
using Valora.Domain.Common.Abstractions;
using Valora.Domain.Common.Results;

namespace Valora.Domain.ValueObjects;

public class RatingScore : ValueObject
{
    public decimal Value { get; }

    private RatingScore(decimal value)
    {
        Value = value;
    }

    public static Result<RatingScore> Create(decimal value, decimal min = 0, decimal max = 5)
    {
        if (value < min || value > max)
        {
            return Result.Failure<RatingScore>(Error.Validation(
                "RatingScore.Invalid",
                $"A nota deve estar entre {min} e {max}."));
        }

        return new RatingScore(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}