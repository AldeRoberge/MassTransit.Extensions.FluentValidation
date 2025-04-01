namespace MassTransit.Extensions.FluentValidation.Tests.Models;

public record TestMessage
{
    // If true, the TestMessageValidator will allow this message to be consumed
    public bool IsValid { get; init; }

    public string Message { get; init; } = "Test message";
}