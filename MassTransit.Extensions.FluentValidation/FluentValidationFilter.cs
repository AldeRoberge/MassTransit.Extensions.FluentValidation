using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MassTransit.Extensions.FluentValidation;

public class FluentValidationFilter<TMessage>(IValidator<TMessage>? validator, IValidationFailurePipe<TMessage> failurePipe) : IFilter<ConsumeContext<TMessage>>
    where TMessage : class
{
    private readonly IValidationFailurePipe<TMessage> _failurePipe = failurePipe ?? throw new ArgumentNullException(nameof(failurePipe));

    public void Probe(ProbeContext context)
    {
        Console.WriteLine("Probing FluentValidationFilter");
        context.CreateScope("FluentValidationFilter");
    }
    
    public async Task Send(
        ConsumeContext<TMessage> context,
        IPipe<ConsumeContext<TMessage>> next)
    {
        if (validator is null)
        {
            Console.WriteLine("Validator is null");
            await next.Send(context);
            return;
        }

        var message = context.Message;
        var validationResult = await validator.ValidateAsync(message, context.CancellationToken);

        if (validationResult.IsValid)
        {
            Console.WriteLine("Validation result is valid");
            await next.Send(context);
            return;
        }

        List<ValidationFailure>? validationProblems = validationResult.Errors;

        var failureContext = new ValidationFailureContext<TMessage>(context, validationProblems);
        Console.WriteLine("Sending failure context");
        await _failurePipe.Send(failureContext);
    }
}