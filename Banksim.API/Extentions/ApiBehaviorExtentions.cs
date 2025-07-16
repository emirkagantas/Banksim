using Microsoft.AspNetCore.Mvc;

namespace BankSim.API.Extensions
{
    public static class ApiBehaviorExtensions
    {
        public static IServiceCollection AddCustomModelStateHandler(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errorMessages = context.ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    var message = string.Join(" ", errorMessages);

                    return new BadRequestObjectResult(new { message });
                };
            });

            return services;
        }
    }
}
