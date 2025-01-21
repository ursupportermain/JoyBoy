using System.Text.Json;
using System.Text.Json.Serialization;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using JoyBoy.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
{
    policy.AllowAnyOrigin()
        .WithMethods(HttpMethod.Get.Method, HttpMethod.Post.Method, HttpMethod.Put.Method, HttpMethod.Delete.Method, HttpMethod.Options.Method)
        .WithHeaders(HeaderNames.ContentType, HeaderNames.Authorization, HeaderNames.AcceptLanguage, "x-api-key", "x-tenant", "x-client-version");
}));

builder.Services.AddSingleton(new DiscordSocketConfig { GatewayIntents = GatewayIntents.All });
builder.Services.AddSingleton<DiscordSocketClient>();
// builder.Services.AddSingleton<AuditLogService>();
builder.Services.AddSingleton(provider =>
{
    var client = provider.GetRequiredService<DiscordSocketClient>();
    return new InteractionService(client, new InteractionServiceConfig());
});
builder.Services.AddHostedService<DiscordService>();
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseCors();
app.MapControllers();
app.Run();