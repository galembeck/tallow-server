using FluentValidation.AspNetCore;

namespace API.Public.Configuration;

public static class ControllersInitializer
{
    public static void ConfigureControllers(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            });

        services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();



        services.AddRouting(options =>
        {
            options.LowercaseUrls = true;
            options.LowercaseQueryStrings = true;
        });
    }
}