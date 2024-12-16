using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JoyBoy.Services;

public class DiscordService : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly InteractionService _interactionService;
    private readonly IServiceProvider _serviceProvider;
    private IGuild? _primaryGuild;

    public DiscordService(
        IConfiguration configuration,
        DiscordSocketClient discordSocketClient,
        InteractionService interactionService,
        IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _discordSocketClient = discordSocketClient;
        _interactionService = interactionService;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), scope.ServiceProvider);

        _discordSocketClient.Ready += ReadyAsync;
        _discordSocketClient.SlashCommandExecuted += SlashCommandExecutedAsync;

        await _discordSocketClient.LoginAsync(TokenType.Bot, _configuration["Discord:Token"]);
        await _discordSocketClient.StartAsync();

        stoppingToken.Register(async () =>
        {
            await _discordSocketClient.StopAsync();
        });
    }

    private async Task SlashCommandExecutedAsync(SocketSlashCommand command)
    {
        var socketInteractionContext = new SocketInteractionContext(_discordSocketClient, command);
        await _interactionService.ExecuteCommandAsync(socketInteractionContext, _serviceProvider);
    }

    private async Task ReadyAsync()
    {
        if (_primaryGuild != null)
            return;

        _primaryGuild = _discordSocketClient.Guilds.FirstOrDefault(x => x.Name == "");

        if (_primaryGuild == null)
            return;

        await _interactionService.RegisterCommandsToGuildAsync(_primaryGuild.Id);
    }
}
