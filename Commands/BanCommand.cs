using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace JoyBoy.Commands
{
    public class BanCommand : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("ban", "Bans a user from the server.")]
        public async Task BanAsync(SocketGuildUser user, string reason = "No reason provided")
        {
            if (Context.User is not SocketGuildUser guildUser || !guildUser.GuildPermissions.BanMembers)
            {
                await RespondAsync("You don't have permission to ban members.", ephemeral: true);
                return;
            }

            if (user == null)
            {
                await RespondAsync("User not found.", ephemeral: true);
                return;
            }

            try
            {
                await user.BanAsync(reason: reason);
                await RespondAsync($"User {user.Username} has been banned. Reason: {reason}");
            }
            catch (System.Exception ex)
            {
                await RespondAsync($"Failed to ban user: {ex.Message}", ephemeral: true);
            }
        }

        [SlashCommand("unban", "Unbans a user from the server.")]
        public async Task UnbanAsync(ulong userId)
        {
            if (Context.User is not SocketGuildUser guildUser || !guildUser.GuildPermissions.BanMembers)
            {
                await RespondAsync("You don't have permission to unban members.", ephemeral: true);
                return;
            }

            try
            {
                await Context.Guild.RemoveBanAsync(userId);
                await RespondAsync($"User with ID {userId} has been unbanned.");
            }
            catch (System.Exception ex)
            {
                await RespondAsync($"Failed to unban user: {ex.Message}", ephemeral: true);
            }
        }

        [SlashCommand("timeout", "Times out a user for a specified duration.")]
        public async Task TimeoutAsync(SocketGuildUser user, int durationMinutes, string reason = "No reason provided")
        {
            if (Context.User is not SocketGuildUser guildUser || !guildUser.GuildPermissions.ModerateMembers)
            {
                await RespondAsync("You don't have permission to timeout members.", ephemeral: true);
                return;
            }

            if (user == null)
            {
                await RespondAsync("User not found.", ephemeral: true);
                return;
            }

            if (durationMinutes <= 0)
            {
                await RespondAsync("Please provide a valid duration in minutes.", ephemeral: true);
                return;
            }

            try
            {
                var timeoutDuration = TimeSpan.FromMinutes(durationMinutes);
                await user.SetTimeOutAsync(timeoutDuration);
                await RespondAsync($"User {user.Username} has been timed out for {durationMinutes} minutes. Reason: {reason}");
            }
            catch (System.Exception ex)
            {
                await RespondAsync($"Failed to timeout user: {ex.Message}", ephemeral: true);
            }
        }
    }
}
