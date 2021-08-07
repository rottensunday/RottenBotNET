namespace NETDiscordBot
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using Microsoft.Extensions.DependencyInjection;
    using Services;

    public class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        // private Program()
        // {
        //     this._client = new DiscordSocketClient();
        //     
        //     _client.Log += Log;
        //     _client.Ready += ReadyAsync;
        //     _client.MessageReceived += MessageReceivedAsync;
        // }
        
        private async Task MainAsync()
        {
            await using var services = ConfigureServices();
            var client = services.GetRequiredService<DiscordSocketClient>();
            client.Log += Log;
            services.GetRequiredService<CommandService>().Log += Log;
            var token = "";
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg);
            
            return Task.CompletedTask;
        }

        // private Task ReadyAsync()
        // {
        //     Console.WriteLine($"{_client.CurrentUser} is connected!");
        //
        //     return Task.CompletedTask;
        // }
        //
        // private async Task MessageReceivedAsync(SocketMessage message)
        // {
        //     if (message.Author.Id == _client.CurrentUser.Id)
        //     {
        //         return;
        //     }
        //
        //     if (message.Content == "!ping")
        //     {
        //         await message.Channel.SendMessageAsync("pong!");
        //     }
        // }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<IDataAccessService, CosmosDbDataAccessService>()
                .BuildServiceProvider();
        }
    }
}