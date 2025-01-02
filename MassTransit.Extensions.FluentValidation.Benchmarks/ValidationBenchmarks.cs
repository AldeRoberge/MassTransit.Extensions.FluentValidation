using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using FluentValidation;
using MassTransit.Extensions.FluentValidation;
using MassTransit.Extensions.FluentValidation.Tests;

[MemoryDiagnoser]
public class ValidationBenchmark
{
    private ITestHarness _harnessWithValidation;
    private ITestHarness _harnessWithoutValidation;
    private IServiceProvider _providerWithValidation;
    private IServiceProvider _providerWithoutValidation;

    [GlobalSetup]
    public async Task Setup()
    {
        _providerWithValidation = SetupProvider(true);
        _providerWithoutValidation = SetupProvider(false);

        _harnessWithValidation = _providerWithValidation.GetRequiredService<ITestHarness>();
        _harnessWithoutValidation = _providerWithoutValidation.GetRequiredService<ITestHarness>();

        await _harnessWithValidation.Start();
        await _harnessWithoutValidation.Start();
    }

    private IServiceProvider SetupProvider(bool withValidation)
    {
        var services = new ServiceCollection();

        if (withValidation)
        {
            services.AddTransient(typeof(IValidationFailurePipe<>), typeof(TestValidationFailurePipe<>));
            services.AddValidatorsFromAssemblyContaining<TestMessageValidator>();
        }

        services.AddMassTransitTestHarness(cfg =>
        {
            cfg.AddConsumer<TestConsumer>();

            cfg.UsingInMemory((context, configurator) =>
            {
                if (withValidation)
                {
                    configurator.UseConsumeFilter(typeof(FluentValidationFilter<>), context);
                }

                configurator.ConfigureEndpoints(context);
            });
        });

        return services.BuildServiceProvider(validateScopes: true);
    }

    [GlobalCleanup]
    public async Task Cleanup()
    {
        if (_harnessWithValidation != null)
            await _harnessWithValidation.Stop();

        if (_harnessWithoutValidation != null)
            await _harnessWithoutValidation.Stop();

        if (_providerWithValidation is IAsyncDisposable disposableWithValidation)
            await disposableWithValidation.DisposeAsync();

        if (_providerWithoutValidation is IAsyncDisposable disposableWithoutValidation)
            await disposableWithoutValidation.DisposeAsync();
    }

    [Benchmark]
    public async Task PublishMessageWithValidation()
    {
        var message = new TestMessage { IsValid = true };
        await _harnessWithValidation.Bus.Publish(message);
        await _harnessWithValidation.Consumed.Any<TestMessage>();
    }

    [Benchmark]
    public async Task PublishMessageWithoutValidation()
    {
        var message = new TestMessage { IsValid = true };
        await _harnessWithoutValidation.Bus.Publish(message);
        await _harnessWithoutValidation.Consumed.Any<TestMessage>();
    }
}