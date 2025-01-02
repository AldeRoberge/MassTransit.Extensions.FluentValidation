using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace MassTransit.Extensions.FluentValidation.Tests;

using System.Threading.Tasks;

public record ValidationResultMessage<TMessage>(TMessage Message, List<ValidationFailure>? ValidationFailures);

public class TestValidationFailurePipe<TMessage> : ValidationFailurePipeBase<TMessage>
    where TMessage : class
{
    public override async Task Send(ValidationFailureContext<TMessage> context)
    {
        var validationProblems = context.ValidationProblems;
        await context.InnerContext.RespondAsync(new ValidationResultMessage<TMessage>(context.InnerContext.Message, validationProblems));
    }
}