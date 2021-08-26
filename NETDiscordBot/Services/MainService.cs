namespace NETDiscordBot.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    public class MainService : IHostedService, IDisposable
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly CommandService _commandService;
        private readonly CommandHandlingService _commandHandlingService;
        private readonly IConfiguration _configuration;
        private readonly StreamingMonitorService _streamingMonitorService;
        
        private string Token => this._configuration["DiscordBotToken"];

        public MainService(
            DiscordSocketClient discordSocketClient,
            CommandService commandService,
            CommandHandlingService commandHandlingService,
            IConfiguration configuration,
            StreamingMonitorService streamingMonitorService)
        {
            this._discordSocketClient = discordSocketClient;
            this._commandService = commandService;
            this._commandHandlingService = commandHandlingService;
            this._configuration = configuration;
            this._streamingMonitorService = streamingMonitorService;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.Log += Log;
            _commandService.Log += Log;
            _discordSocketClient.UserVoiceStateUpdated += this._streamingMonitorService.Handle;
            await _discordSocketClient.LoginAsync(TokenType.Bot, Token);
            await _discordSocketClient.StartAsync();
            await _commandHandlingService.InitializeAsync();
            await Task.Delay(-1, cancellationToken);
        }
        
        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
        
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg);
            
            return Task.CompletedTask;
        }
    }
}