using System.Security.Claims;
using System.Threading.RateLimiting;

namespace ReverseProxy.Extensions;

public static class RateLimitExtensions
{
    private static readonly string Policy = "PerUserRatelimit";

    public static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        return services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy(Policy, context =>
            {
                // We always have a user id
                var id = context.User.FindFirstValue("id")!;

                return RateLimitPartition.GetTokenBucketLimiter(id, key =>
                    new TokenBucketRateLimiterOptions
                    {
                        ReplenishmentPeriod = TimeSpan.FromSeconds(5),
                        AutoReplenishment = true,
                        TokenLimit = 2,
                        TokensPerPeriod = 2,
                        QueueLimit = 2,
                    });
            });
        });
    }

    public static IEndpointConventionBuilder RequirePerUserRateLimit(this IEndpointConventionBuilder builder)
    {
        return builder.RequireRateLimiting(Policy);
    }
}