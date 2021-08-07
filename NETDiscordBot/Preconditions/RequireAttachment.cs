namespace NETDiscordBot.Preconditions
{
    using System;
    using System.Threading.Tasks;
    using Discord.Commands;

    public class RequireAttachment : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(
            ICommandContext context, 
            CommandInfo command,
            IServiceProvider services)
            => context.Message.Attachments.Count switch
            {
                0 => Task.Run(() => PreconditionResult.FromError("Given message has no attachment")),
                _ => Task.Run(PreconditionResult.FromSuccess)
            };
    }
}