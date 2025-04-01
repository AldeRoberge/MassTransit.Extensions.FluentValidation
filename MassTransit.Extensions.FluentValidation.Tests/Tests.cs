using MassTransit.Extensions.FluentValidation.Tests.Models;
using Shouldly;

namespace MassTransit.Extensions.FluentValidation.Tests
{
    /// <summary>
    /// A test class that demonstrates how to use the MassTransit test harness with the FluentValidation filter.
    /// </summary>
    public class Tests : TestBase
    {
        [Test]
        public async Task Should_Consume_Message_Successfully()
        {
            var message = new TestMessage
            {
                IsValid = true,
                Message = "This message is valid and should be consumed.",
            };

            await Harness.Bus.Publish(message);

            // Assert that the consumer consumed the message
            (await Harness.Consumed.Any<TestMessage>()).ShouldBeTrue();
            var consumerHarness = Harness.GetConsumerHarness<TestMessageConsumer>();
            (await consumerHarness.Consumed.Any<TestMessage>()).ShouldBeTrue();
        }

        [Test]
        public async Task Should_Not_Consume_Invalid_Message()
        {
            var message = new TestMessage
            {
                IsValid = false,
                Message = "This message is not valid and should not be consumed.",
            };

            await Harness.Bus.Publish(message);

            // Assert that the consumer did not consume the invalid message
            var consumerHarness = Harness.GetConsumerHarness<TestMessageConsumer>();
            (await consumerHarness.Consumed.Any<TestMessage>()).ShouldBeFalse();
            (await Harness.Consumed.Any<TestMessage>()).ShouldBeTrue(); // Message reached the bus
        }

        [Test]
        public async Task Should_Not_Consume_Invalid_Message_With_No_Message()
        {
            var message = new TestMessage
            {
                IsValid = true,
                Message = string.Empty, // Invalid message
            };

            await Harness.Bus.Publish(message);

            // Assert that the consumer did not consume the invalid message
            var consumerHarness = Harness.GetConsumerHarness<TestMessageConsumer>();

            (await consumerHarness.Consumed.Any<TestMessage>()).ShouldBeFalse();

            // Assert that the message was not consumed by the consumer
            (await Harness.Consumed.Any<TestMessage>()).ShouldBeTrue(); // Message reached the bus

            // Assert that the message was not consumed by the consumer
            var exception = consumerHarness.Consumed.Select<TestMessage>().FirstOrDefault();

            exception.ShouldNotBeNull();
            
            // Get the response (ValidationFailure) from the consumer
            
        }
    }
}