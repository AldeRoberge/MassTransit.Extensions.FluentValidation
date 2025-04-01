using System.Collections.Generic;

namespace MassTransit.Extensions.FluentValidation.DefaultValidationFailurePipe;

public record ValidationFailure<TMessage>(TMessage Message, List<ValidationFailure>? ValidationFailures);