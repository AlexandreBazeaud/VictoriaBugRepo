using System;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Victoria;

namespace VictoriaBug
{
    public class Initialize
    {
        private readonly CommandService _commands;
        private readonly DiscordShardedClient _client;
        public Initialize(CommandService commands = null, DiscordShardedClient client = null)
        {
            _commands = commands ?? new CommandService();
            _client = client ?? new DiscordShardedClient();
        }
        
        public IServiceProvider BuildServiceProvider() => new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_commands)
            .AddLogging()
            // You can pass in an instance of the desired type
            .AddLavaNode(x =>
            {
                x.SelfDeaf = true;
                x.Port = 2335;
                x.Hostname = "localhost";
                x.Authorization = "youshallnotpass";
            })
            // ...or by using the generic method.
            //
            // The benefit of using the generic method is that 
            // ASP.NET DI will attempt to inject the required
            // dependencies that are specified under the constructor 
            // for us.
            .AddSingleton<CommandHandler>()
            .BuildServiceProvider();
    }
}