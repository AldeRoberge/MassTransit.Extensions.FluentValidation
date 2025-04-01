using MassTransit.Extensions.FluentValidation.DefaultValidationFailurePipe;
using Microsoft.Extensions.DependencyInjection;

namespace MassTransit.Extensions.FluentValidation;

/// <summary>
/// Extension methods for injecting functionality from the <see cref="FluentValidation"/> library into a <see cref="MassTransit"/> pipeline.
/// </summary>
public static class FluentValidationExtensions
{
    /// <summary>
    /// Registers the necessary filters to perform validation on message sent through the pipeline.
    /// </summary>
    public static IBusFactoryConfigurator UseFluentValidationForMassTransit(this IBusFactoryConfigurator configurator, IRegistrationContext context)
    {
        configurator.UseConsumeFilter(typeof(FluentValidationFilter<>), context);
        return configurator;
    }

    public static IServiceCollection AddDefaultMassTransitValidationFailurePipe(this IServiceCollection services)
    {
        // Registers a simple failure pipe that simply responds with ValidationFailure when the validation fails.
        services.AddTransient(
            typeof(IValidationFailurePipe<>),
            typeof(TestValidationFailurePipe<>));
        return services;
    }
}