namespace MassTransit.Extensions.FluentValidation.Tests;

public record TestMessage
{
    // If true, the validator will pass the test (see TestMessageValidator.cs)
    public bool IsValid { get; init; }
}