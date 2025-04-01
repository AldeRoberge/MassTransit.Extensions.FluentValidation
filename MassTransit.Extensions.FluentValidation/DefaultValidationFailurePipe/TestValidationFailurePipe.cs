using System.Threading.Tasks;

namespace MassTransit.Extensions.FluentValidation.DefaultValidationFailurePipe;

public class TestValidationFailurePipe<TMessage> : ValidationFailurePipeBase<TMessage>
    where TMessage : class
{
    public override async Task Send(ValidationFailureContext<TMessage> context)
    {
        var errorResponse = new ValidationFailure<TMessage>(context.InnerContext.Message, context.ValidationProblems);
        await context.InnerContext.RespondAsync(errorResponse);
    }
}