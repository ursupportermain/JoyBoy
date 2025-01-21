using Discord.Interactions;


namespace JoyBoy.Commands
{
    public class SlashCommandHandler : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("waifu", "get ur waifu")]
        public async Task PingAsync()
        {
            string gifUrl = "https://tenor.com/view/go-toubun-no-hanayome-anime-ichika-nino-miku-gif-17499272"; // Beispiel-GIF
            await RespondAsync($"{gifUrl}");
        }
    }
}