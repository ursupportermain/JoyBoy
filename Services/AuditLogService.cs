// using Discord;
// using Discord.WebSocket;
// using System;
// using System.Linq;
// using System.Threading.Tasks;
//
// namespace JoyBoy.Services
// {
//     public class AuditLogService
//     {
//         private readonly DiscordSocketClient _client;
//         private readonly ulong _auditLogChannelId = 123456789012345678; // Example channel ID
//
//         public AuditLogService(DiscordSocketClient client, ulong auditLogChannelId)
//         {
//             _client = client;
//             _auditLogChannelId = auditLogChannelId;
//             _client.UserBanned += OnUserBannedAsync;
//             _client.UserUnbanned += OnUserUnbannedAsync;
//             _client.UserUpdated += OnUserTimeoutAsync;
//             _client.GuildAvailable += OnGuildAvailableAsync;
//         }
//
//         private async Task OnUserBannedAsync(SocketUser user, SocketGuild guild)
//         {
//             var channel = guild.GetTextChannel(_auditLogChannelId);
//             if (channel != null)
//             {
//                 await channel.SendMessageAsync($":hammer: **{user.Username}#{user.Discriminator}** was banned from the server.");
//             }
//         }
//
//         private async Task OnUserUnbannedAsync(SocketUser user, SocketGuild guild)
//         {
//             var channel = guild.GetTextChannel(_auditLogChannelId);
//             if (channel != null)
//             {
//                 await channel.SendMessageAsync($":unlock: **{user.Username}#{user.Discriminator}** was unbanned from the server.");
//             }
//         }
//
//         private async Task OnUserTimeoutAsync(SocketUser before, SocketUser after)
//         {
//             if (after is SocketGuildUser updatedUser && updatedUser.TimedOutUntil.HasValue)
//             {
//                 var channel = updatedUser.Guild.GetTextChannel(_auditLogChannelId);
//                 if (channel != null)
//                 {
//                     await channel.SendMessageAsync($":clock1: **{updatedUser.Username}#{updatedUser.Discriminator}** was timed out until {updatedUser.TimedOutUntil.Value:F} UTC.");
//                 }
//             }
//         }
//
//         private async Task OnGuildAvailableAsync(SocketGuild guild)
//         {
//             var channel = guild.GetTextChannel(_auditLogChannelId);
//             if (channel == null) return;
//
//             try
//             {
//                 var auditLogs = await guild.GetAuditLogsAsync(5).FlattenAsync();
//                 foreach (var entry in auditLogs)
//                 {
//                     string action = entry.Action switch
//                     {
//                         ActionType.Ban => $":hammer: User ID **{entry.TargetId}** was banned",
//                         ActionType.Unban => $":unlock: User ID **{entry.TargetId}** was unbanned",
//                         ActionType.MemberUpdated => $":pencil: A user was updated",
//                         _ => null
//                     };
//
//                     if (action != null)
//                     {
//                         await channel.SendMessageAsync($"{action} by {entry.User?.Username ?? "Unknown"}#{entry.User?.Discriminator ?? "0000"}");
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine($"Failed to fetch audit logs: {ex.Message}");
//             }
//         }
//     }
// }
