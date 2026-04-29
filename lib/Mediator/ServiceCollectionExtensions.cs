using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator;

/// <summary>
/// DI registration helpers for mediator and handlers.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the mediator and scans for handlers in the specified assemblies.
    /// </summary>
    public static IServiceCollection AddMediator(this IServiceCollection services, params Assembly[] assembliesToScan)
    {
        services.AddScoped<IMediator, Mediator>();

        var assemblies = assembliesToScan?.Length > 0 ? assembliesToScan : AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            RegisterHandlers(services, assembly);
        }

        return services;
    }

    private static void RegisterHandlers(IServiceCollection services, Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (type.IsAbstract || type.IsInterface)
            {
                continue;
            }

            foreach (var @interface in type.GetInterfaces())
            {
                // IRequestHandler<TRequest, TResponse>
                if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                {
                    RegisterService(services, @interface, type);
                }

                // INotificationHandler<TNotification>
                if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(INotificationHandler<>))
                {
                    RegisterService(services, @interface, type);
                }

                // IPipelineBehavior<TRequest, TResponse>
                if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>))
                {
                    RegisterService(services, @interface, type);
                }
            }
        }
    }

    private static void RegisterService(IServiceCollection services, Type serviceType, Type implementationType)
    {
        if (implementationType.IsGenericTypeDefinition)
        {
            services.AddTransient(serviceType.GetGenericTypeDefinition(), implementationType);
            return;
        }

        services.AddTransient(serviceType, implementationType);
    }
}
