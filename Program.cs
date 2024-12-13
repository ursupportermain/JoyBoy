using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace JoyBoy
{
    class Program
    {
        private DiscordSocketClient client;

        static void Main(string[] args)
            => new Program().RunBotAsync().GetAwaiter().GetResult();

        public async Task RunBotAsync()
        {
            client = new DiscordSocketClient();

            client.Log += Log;
            client.Ready += OnReadyAsync;

            try
            {
                var token = LoadToken();

                await client.LoginAsync(TokenType.Bot, token);
                await client.StartAsync();

                await Task.Delay(-1);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Make sure secrets.json exists in the correct location.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }

        private string LoadToken()
        {
            var secretsPath = Path.Combine( "secrets.json");

            if (!File.Exists(secretsPath))
                throw new FileNotFoundException($"The file '{secretsPath}' was not found.");

            var json = File.ReadAllText(secretsPath);
            var secrets = JsonSerializer.Deserialize<Secrets>(json);

            if (secrets == null || string.IsNullOrEmpty(secrets.BotToken))
                throw new Exception("Invalid or missing BotToken in secrets.json.");

            return secrets.BotToken;
        }

        private Task Log(LogMessage log)
        {
            Console.WriteLine(log);
            return Task.CompletedTask;
        }

        private Task OnReadyAsync()
        {
            Console.WriteLine($"Bot connected as {client.CurrentUser.Username}#{client.CurrentUser.Discriminator}");
            return Task.CompletedTask;
        }

        private class Secrets
        {
            public string BotToken { get; set; }
        }
    }
}
