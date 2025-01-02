using System;
using System.Collections.Generic;

namespace MassTransit.Extensions.FluentValidation;

public class ValidationFailureContext<TMessage>(ConsumeContext<TMessage> wrappedContext, List<ValidationFailure>? validationFailures) :
    BasePipeContext(wrappedContext.CancellationToken)
    where TMessage : class
{
    public ConsumeContext<TMessage> InnerContext { get; } = wrappedContext ?? throw new ArgumentNullException(nameof(wrappedContext));
    public List<ValidationFailure>? ValidationProblems { get; } = validationFailures ?? throw new ArgumentNullException(nameof(validationFailures));
}