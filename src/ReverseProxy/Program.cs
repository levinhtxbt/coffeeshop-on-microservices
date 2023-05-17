using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using N8T.Infrastructure.OTel;
using ReverseProxy.Extensions;
using Steeltoe.Discovery.Client;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.AddOTelLogs()
    .ConfigureKestrel(webBuilder =>
    {
        webBuilder.Listen(IPAddress.Any, builder.Configuration.GetValue("RestPort", 5000)); // REST
    });

builder.Configuration.AddYamlFile("appsettings.yml", optional: false, reloadOnChange: true);
builder.Services.AddDiscoveryClient();
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    /* 4. Service Discovery */
    .LoadFromDiscoveryService()
    /* 3. Authorization */
    .AddTransforms(context =>
    {
        if (string.Equals("myPolicy", context.Route.AuthorizationPolicy))
        {
            context.AddRequestTransform(async transformContext =>
            {
                // AuthN and AuthZ will have already been completed after request routing.
                var ticket =
                    await transformContext.HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);

                // Reject invalid requests
                if (ticket.Principal?.Claims.Count()! <= 0)
                {
                    var response = transformContext.HttpContext.Response;
                    response.StatusCode = 401;
                }
            });
        }
    });

builder.Services.AddRateLimiting();
builder.Services.AddOTelTracing(builder.Configuration);
builder.Services.AddOTelMetrics(builder.Configuration);

// 3. Authorization
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateIssuerSigningKey = false,
        SignatureValidator = delegate(string token, TokenValidationParameters _)
        {
            var jwt = new JwtSecurityToken(token);
            return jwt;
        }
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("mypolicy", builder => builder
        .RequireClaim("myCustomClaim", "green")
        .RequireAuthenticatedUser());
    options.FallbackPolicy = null;
});


var app = builder.Build();

app.UseRateLimiter();

app.MapReverseProxy()
    .RequirePerUserRateLimit();

// Configure the prometheus endpoint for scraping metrics
// app.MapPrometheusScrapingEndpoint()
// NOTE: This should only be exposed on an internal port!
// .RequireHost("*:9090");

app.Run();