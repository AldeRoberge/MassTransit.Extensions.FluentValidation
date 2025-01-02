using FluentValidation;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace MassTransit.Extensions.FluentValidation.Tests
{
    /// <summary>
    /// A test class that demonstrates how to use the MassTransit test harness with the FluentValidation filter.
    /// </summary>
    public class Tests
    {
        private ITestHarness?     _harness;
        private IServiceProvider? _provider;

        [SetUp]
        public async Task Setup()
        {
            var services = new ServiceCollection();

            services.AddTransient(
                typeof(IValidationFailurePipe<>),
                typeof(TestValidationFailurePipe<>));

            // Register FluentValidation validators
            services.AddValidatorsFromAssemblyContaining<TestMessageValidator>();

            services.AddMassTransitTestHarness(cfg =>
            {
                // Register the consumer
                cfg.AddConsumer<TestConsumer>();

                // Configure the in-memory bus
                cfg.UsingInMemory((context, configurator) =>
                {
                    // This works :
                    configurator.UseConsumeFilter(typeof(FluentValidationFilter<>), context);

                    configurator.ReceiveEndpoint(endpointConfigurator =>
                    {
                        // The following does not work :
                        // endpointConfigurator.UseFluentValidationForMassTransit(context);
                        // endpointConfigurator.UseConsumeFilter(typeof(FluentValidationFilter<>), context);
                    });
                    configurator.ConfigureEndpoints(context);
                });
            });


            // Build the service provider
            _provider = services.BuildServiceProvider(validateScopes: true);

            // Retrieve the test harness
            _harness = _provider.GetRequiredService<ITestHarness>();

            // Start the harness
            await _harness.Start();
        }


        [Test]
        public async Task Should_Validate_Message_Successfully()
        {
            var message = new TestMessage { IsValid = true };

            await _harness!.Bus.Publish(message);

            // Assert that the consumer consumed the message
            (await _harness.Consumed.Any<TestMessage>()).ShouldBeTrue();
            var consumerHarness = _harness.GetConsumerHarness<TestConsumer>();
            (await consumerHarness.Consumed.Any<TestMessage>()).ShouldBeTrue();
        }

        [Test]
        public async Task Should_Handle_Validation_Failure()
        {
            var message = new TestMessage { IsValid = false };

            await _harness!.Bus.Publish(message);

            // Assert that the consumer did not consume the invalid message
            var consumerHarness = _harness.GetConsumerHarness<TestConsumer>();
            (await consumerHarness.Consumed.Any<TestMessage>()).ShouldBeFalse();
            (await _harness.Consumed.Any<TestMessage>()).ShouldBeTrue(); // Message reached the bus
        }


        [TearDown]
        public async Task TearDown()
        {
            if (_harness != null)
                await _harness.Stop();

            if (_provider is IAsyncDisposable disposable)
                await disposable.DisposeAsync();
        }
    }
}