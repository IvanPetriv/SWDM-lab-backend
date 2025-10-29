using Microsoft.OpenApi.Models;

namespace Api.OpenApi;

public static class Extensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        return services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "API",
                Description = "REST API",
            });
        });
    }
}