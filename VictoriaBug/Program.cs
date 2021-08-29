// See https://aka.ms/new-console-template for more information

using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Victoria.Node;

namespace VictoriaBug
{
    class Program
    {
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            // You should dispose a service provider created using ASP.NET
            // when you are finished using it, at the end of your app's lifetime.
            // If you use another dependency injection framework, you should inspect
            // its documentation for the best way to do this.
            var a = new Initialize();
            
            using (_serviceProvider = a.BuildServiceProvider() as ServiceProvider)
            {
                _client = _serviceProvider.GetRequiredService<DiscordShardedClient>();

                _client.Log += LogAsync;
                _client.ShardReady += ShardReadyAsync;
                _serviceProvider.GetRequiredService<CommandService>().Log += LogAsync;

                // Tokens should be considered secret data and never hard-coded.
                // We can read from the environment variable to avoid hardcoding.
                await _client.LoginAsync(TokenType.Bot, "");
                await _client.StartAsync();

                // Here we initialize the logic required to register our commands.
                await _serviceProvider.GetRequiredService<CommandHandler>().InitializeAsync();

                await Task.Delay(Timeout.Infinite);
            }
        }

        private DiscordShardedClient _client;
        private ServiceProvider _serviceProvider;
        private int shardReady = 0;
        private async Task ShardReadyAsync(DiscordSocketClient pClient)
        {
            shardReady++;
            if (shardReady != _client.Shards.Count) return;

            await _serviceProvider.GetRequiredService<LavaNode>().ConnectAsync();
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }
    }
}