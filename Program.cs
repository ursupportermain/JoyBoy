using Microsoft.Extensions.Configuration;
using JoyBoy.Services;

namespace JoyBoy
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var discordService = new DiscordService(configuration);
            await discordService.StartAsync();
        }
    }
}