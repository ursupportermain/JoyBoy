using Discord.Interactions;


namespace JoyBoy.Commands
{
    public class SlashCommandHandler : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("ping", "Responds with Pong!")]
        public async Task PingAsync()
        {
            await RespondAsync("Pong!");
        }
    }
}