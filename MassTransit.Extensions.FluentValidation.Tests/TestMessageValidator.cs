using FluentValidation;
using Shouldly;

namespace MassTransit.Extensions.FluentValidation.Tests;

public class TestMessageValidator : AbstractValidator<TestMessage>
{
    public TestMessageValidator()
    {
        // Simple validation to ensure the IsValid property is true (allows to fail validation for testing purposes)
        RuleFor(x => x.IsValid)
            .Equal(true)
            .WithMessage($"The {nameof(TestMessage.IsValid)} value must be true.");
    }
}