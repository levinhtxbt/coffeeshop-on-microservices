var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddYamlFile("appsettings.yml", optional: false, reloadOnChange: true);
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapReverseProxy();
app.Run();
