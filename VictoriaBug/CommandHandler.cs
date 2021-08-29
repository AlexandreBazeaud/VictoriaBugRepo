using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace VictoriaBug
{
    public class CommandHandler
    {
        private readonly DiscordShardedClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        public CommandHandler(IServiceProvider services, CommandService commands, DiscordShardedClient client)
        {
            _commands = commands;
            _services = services;
            _client = client;
        }
        
        public async Task InitializeAsync()
        {
            // Pass the service provider to the second parameter of
            // AddModulesAsync to inject dependencies to all modules 
            // that may require them.
            await _commands.AddModulesAsync(
                assembly: Assembly.GetEntryAssembly(), 
                services: _services);
            _client.MessageReceived += HandleCommandAsync;
        }

        public async Task HandleCommandAsync(SocketMessage msg)
        {
            var message = msg as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix('!', ref argPos) || 
                  message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new ShardedCommandContext(_client, message);
            // ...
            // Pass the service provider to the ExecuteAsync method for
            // precondition checks.
            await _commands.ExecuteAsync(
                context: context, 
                argPos: argPos, 
                services: _services);
            // ...
        }
    }
}