using FluentValidation;

namespace MassTransit.Extensions.FluentValidation.Tests.Models;

public class TestMessageValidator : AbstractValidator<TestMessage>
{
    public TestMessageValidator()
    {
        // Simple validation to ensure the IsValid property is true (allows to fail validation for testing purposes)
        RuleFor(x => x.IsValid)
            .Equal(true)
            .WithMessage($"The {nameof(TestMessage.IsValid)} value must be true.");

        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage($"The {nameof(TestMessage.Message)} value cannot be empty.");
    }
}