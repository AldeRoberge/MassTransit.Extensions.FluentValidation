using FluentValidation;
using MassTransit.Extensions.FluentValidation.Tests.Models;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace MassTransit.Extensions.FluentValidation.Tests;

public class TestBase
{
    internal ITestHarness Harness;

    private IServiceProvider _provider;

    [SetUp]
    public async Task Setup()
    {
        var services = new ServiceCollection();

        // The default MassTransit validation failure pipe
        // Simply responds with ValidationFailure when validation fails
        services.AddDefaultMassTransitValidationFailurePipe();

        // Register FluentValidation validators
        services.AddValidatorsFromAssemblyContaining<TestMessageValidator>();

        services.AddMassTransitTestHarness(cfg =>
        {
            // Register the consumer
            cfg.AddConsumer<TestMessageConsumer>();

            // Configure the in-memory bus
            cfg.UsingInMemory((context, configurator) =>
            {
                // The consume filter that validates
                configurator.UseFluentValidationForMassTransit(context);

                configurator.ConfigureEndpoints(context);
            });
        });

        // Build the service provider
        _provider = services.BuildServiceProvider(validateScopes: true);

        // Retrieve the test harness
        Harness = _provider.GetRequiredService<ITestHarness>();

        // Start the harness
        await Harness.Start();
    }

    [TearDown]
    public async Task TearDown()
    {
        if (Harness != null)
            await Harness.Stop();

        if (_provider is IAsyncDisposable disposable)
            await disposable.DisposeAsync();
    }
}