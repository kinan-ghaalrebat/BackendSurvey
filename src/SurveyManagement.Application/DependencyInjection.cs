using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace SurveyManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // Finds every IRequestHandler (commands + queries) in this project
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // Finds every AbstractValidator<T> in this project
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
