using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;


namespace JoyBoy.Services;

    public class DiscordService
    {
        private readonly DiscordSocketClient _client;
        private readonly string _token;

        public DiscordService(IConfiguration configuration)
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;
            _client.Ready += OnReadyAsync;

            _token = configuration["BotToken"] ?? throw new Exception("BotToken is missing in appsettings.json.");
        }

        public async Task StartAsync()
        {
            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();
            Console.WriteLine("Discord bot started.");

            await Task.Delay(-1); 
        }

        private Task Log(LogMessage log)
        {
            Console.WriteLine(log);
            return Task.CompletedTask;
        }

        private Task OnReadyAsync()
        {
            Console.WriteLine($"Bot connected as {_client.CurrentUser.Username}#{_client.CurrentUser.Discriminator}");
            return Task.CompletedTask;
        }
    }
