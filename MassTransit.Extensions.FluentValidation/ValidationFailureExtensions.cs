using System.Collections.Generic;
using System.Linq;

namespace MassTransit.Extensions.FluentValidation;

public static class ValidationFailureExtensions
{
    public static IDictionary<string, string[]> ToErrorDictionary(this IEnumerable<ValidationFailure> validationProblems)
    {
        return validationProblems
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(failure => failure.ErrorMessage).ToArray());
    }
}
