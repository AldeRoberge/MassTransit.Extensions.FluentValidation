namespace MassTransit.Extensions.FluentValidation.Tests;

public class TestConsumer : IConsumer<TestMessage>
{
    public Task Consume(ConsumeContext<TestMessage> context)
    {
        Console.WriteLine("Message consumed");

        // Handle the message
        return Task.CompletedTask;
    }
}