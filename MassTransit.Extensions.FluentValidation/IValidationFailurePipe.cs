namespace MassTransit.Extensions.FluentValidation;

public interface IValidationFailurePipe<TMessage> :
    IPipe<ValidationFailureContext<TMessage>>
    where TMessage : class;