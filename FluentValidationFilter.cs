using MassTransit;

namespace FluentValidationForMassTransit;

public class FluentValidationFilter<TMessage>(IValidator<TMessage>? validator, IValidationFailurePipe<TMessage> failurePipe) : IFilter<ConsumeContext<TMessage>>
    where TMessage : class
{
    private readonly IValidationFailurePipe<TMessage> _failurePipe = failurePipe ?? throw new ArgumentNullException(nameof(failurePipe));

    public void Probe(ProbeContext context)
    {
        context.CreateScope("FluentValidationFilter");
    }
    
    public async Task Send(
        ConsumeContext<TMessage> context,
        IPipe<ConsumeContext<TMessage>> next)
    {
        if (validator is null)
        {
            await next.Send(context);
            return;
        }

        var message = context.Message;
        var validationResult = await validator.ValidateAsync(message, context.CancellationToken);

        if (validationResult.IsValid)
        {
            await next.Send(context);
            return;
        }

        var validationProblems = validationResult.Errors.ToErrorDictionary();

        var failureContext = new ValidationFailureContext<TMessage>(context, validationProblems);
        await _failurePipe.Send(failureContext);
    }
}