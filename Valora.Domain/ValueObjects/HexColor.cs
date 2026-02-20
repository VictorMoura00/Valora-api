using System.Collections.Generic;
using System.Text.RegularExpressions;
using Valora.Domain.Common.Abstractions;
using Valora.Domain.Common.Results;

namespace Valora.Domain.ValueObjects;

public class HexColor : ValueObject
{
    public string Value { get; }

    private HexColor(string value) => Value = value;

    public static Result<HexColor> Create(string hexCode)
    {
        if (string.IsNullOrWhiteSpace(hexCode))
            return Result.Failure<HexColor>(Error.Validation("Color.Empty", "A cor não pode ser vazia."));

        // SRP: A regra do que é uma cor hexadecimal vive apenas aqui
        if (!Regex.IsMatch(hexCode, "^#(?:[0-9a-fA-F]{3}){1,2}$"))
            return Result.Failure<HexColor>(Error.Validation("Color.Invalid", "Formato hexadecimal inválido. Use algo como #FF0000."));

        return new HexColor(hexCode.ToUpperInvariant());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}