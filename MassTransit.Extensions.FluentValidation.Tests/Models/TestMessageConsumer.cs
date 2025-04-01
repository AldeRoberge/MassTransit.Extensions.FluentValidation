namespace MassTransit.Extensions.FluentValidation.Tests.Models;

public class TestMessageConsumer : IConsumer<TestMessage>
{
    public Task Consume(ConsumeContext<TestMessage> context)
    {
        Console.WriteLine("");
        Console.WriteLine($"Message consumed with content: '{context.Message.Message}'.");
        Console.WriteLine("");
        return Task.CompletedTask;
    }
}