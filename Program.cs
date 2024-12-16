using System.Text.Json;
using System.Text.Json.Serialization;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Net.Http.Headers;
using JoyBoy.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddCors(options => options.AddDefaultPolicy(builder =>
{
    builder.AllowAnyOrigin();
    builder.WithMethods(HttpMethod.Get.Method, HttpMethod.Post.Method, HttpMethod.Put.Method, HttpMethod.Delete.Method, HttpMethod.Options.Method);
    builder.WithHeaders(HeaderNames.ContentType, HeaderNames.Authorization, HeaderNames.AcceptLanguage, "x-api-key", "x-tenant", "x-client-version");
}));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var discordSocketConfig = new DiscordSocketConfig
{
    GatewayIntents = Discord.GatewayIntents.All
};

var interactionServiceConfig = new InteractionServiceConfig
{

};

builder.Services.AddSingleton(discordSocketConfig);
builder.Services.AddSingleton<DiscordSocketClient>();
builder.Services.AddSingleton(interactionServiceConfig);
builder.Services.AddSingleton<InteractionService>();
builder.Services.AddHostedService<DiscordService>();


builder.Services.AddSingleton(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var botToken = configuration["BotToken"];

    if (string.IsNullOrEmpty(botToken))
        throw new InvalidOperationException("BotToken is missing in appsettings.json.");

    return botToken;
});

var app = builder.Build();

app.UseCors();

app.MapControllers();
app.Run();
