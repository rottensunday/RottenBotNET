namespace NETDiscordBot.Services
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using Microsoft.Extensions.DependencyInjection;

    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;

        public CommandHandlingService(IServiceProvider services)
        {
            this._commands = services.GetRequiredService<CommandService>();
            this._discord = services.GetRequiredService<DiscordSocketClient>();
            this._services = services;

            this._commands.CommandExecuted += CommandExecutedAsync;
            this._discord.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync()
        {
            await this._commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            if (rawMessage is not SocketUserMessage { Source: MessageSource.User } message)
            {
                return;
            }

            var argPos = 0;

            if (!message.HasCharPrefix('!', ref argPos))
            {
                return;
            }

            var context = new SocketCommandContext(this._discord, message);

            await this._commands.ExecuteAsync(context, argPos, _services);
        }

        private async Task CommandExecutedAsync(
            Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!command.IsSpecified
                || result.IsSuccess)
            {
                return;
            }

            await context.Channel.SendMessageAsync($"error: {result}");
        }
    }
}