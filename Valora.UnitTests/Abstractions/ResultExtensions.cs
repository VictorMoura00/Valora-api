using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Valora.Domain.Common.Results;

namespace Valora.UnitTests.Abstractions;

public static class ResultExtensions
{
    public static ResultAssertions Should(this Result instance)
    {
        return new ResultAssertions(instance);
    }
}

public class ResultAssertions(Result subject) 
    : ReferenceTypeAssertions<Result, ResultAssertions>(subject, AssertionChain.GetOrCreate())
{
    protected override string Identifier => "Result";

    public AndConstraint<ResultAssertions> BeSuccess(string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.IsSuccess)
            .FailWith("Esperava que o resultado fosse sucesso, mas falhou com erro: {0}", Subject.Error);

        return new AndConstraint<ResultAssertions>(this);
    }

    public AndConstraint<ResultAssertions> BeFailure(Error expectedError, string because = "", params object[] becauseArgs)
    {
        CurrentAssertionChain
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.IsFailure)
            .FailWith("Esperava falha, mas foi sucesso.")
            .Then
            .ForCondition(Subject.Error.Equals(expectedError) || Subject.Error.Code == expectedError.Code)
            .FailWith("Esperava erro '{0}', mas encontrou '{1}'", expectedError.Code, Subject.Error.Code);

        return new AndConstraint<ResultAssertions>(this);
    }
}