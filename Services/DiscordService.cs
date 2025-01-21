using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JoyBoy.Services;

public class DiscordService(
    IConfiguration configuration,
    DiscordSocketClient discordSocketClient,
    IServiceProvider serviceProvider)
    : BackgroundService
{
    private readonly InteractionService _interactionService = new InteractionService(discordSocketClient, new InteractionServiceConfig());
    private IGuild? _primaryGuild;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), scope.ServiceProvider);

        discordSocketClient.Ready += ReadyAsync;
        discordSocketClient.SlashCommandExecuted += SlashCommandExecutedAsync;

        try
        {
            var token = configuration["Discord:Token"];
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("Bot token is missing from configuration.");
            }

            await discordSocketClient.LoginAsync(TokenType.Bot, token);
            await discordSocketClient.StartAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while starting Discord client: {ex.Message}");
            throw;
        }

        stoppingToken.Register(async () =>
        {
            await discordSocketClient.StopAsync();
        });
    }

    private async Task SlashCommandExecutedAsync(SocketSlashCommand command)
    {
        var socketInteractionContext = new SocketInteractionContext(discordSocketClient, command);
        try
        {
            await _interactionService.ExecuteCommandAsync(socketInteractionContext, serviceProvider);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing slash command: {ex.Message}");
            await command.RespondAsync("An error occurred while executing the command.", ephemeral: true);
        }
    }

    private async Task ReadyAsync()
    {
        if (_primaryGuild != null)
            return;

        _primaryGuild = discordSocketClient.Guilds.FirstOrDefault(x => x.Name == "IT11a");

        if (_primaryGuild == null)
        {
            Console.WriteLine("Primary guild not found.");
            return;
        }

        try
        {
            await _interactionService.RegisterCommandsToGuildAsync(_primaryGuild.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error registering commands to guild: {ex.Message}");
        }
    }
}
