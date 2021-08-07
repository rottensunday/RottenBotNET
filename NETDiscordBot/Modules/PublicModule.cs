namespace NETDiscordBot.Modules
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Discord.Commands;
    using Models;
    using Preconditions;
    using Services;

    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        private readonly IDataAccessService _dataAccess;

        public PublicModule(
            IDataAccessService dataAccess)
        {
            this._dataAccess = dataAccess;
        }

        [Command("ping")]
        [Alias("pong", "hello")]
        public Task PingAsync()
        {
            return this.ReplyAsync("pong!!");
        }

        [Command("save")]
        [Alias("s", "sa", "sav")]
        [RequireAttachment]
        public async Task SaveAttachmentAsync(string key)
        {
            var attachmentUrl = this.Context.Message.Attachments.First().Url;
            Console.WriteLine($"Saving attachment with key: {key}, url: {attachmentUrl}");
            
            await this._dataAccess.SaveAttachmentEntry(new AttachmentEntry(key, attachmentUrl));
            await this.Context.Channel.SendMessageAsync("Dodano wpis mordo");
            Console.WriteLine("Succesfully saved attachment");
        }

        [Command("load")]
        [Alias("l", "lo", "loa")]
        public async Task LoadAttachmentAsync(string key)
        {
            Console.WriteLine($"Loading attachment with key: {key}");

            try
            {
                var result = await this._dataAccess.LoadAttachmentEntry(key);
                await this.Context.Channel.SendMessageAsync(result.Url);
                Console.WriteLine("Succesfully loaded attachment");
            }
            catch
            {
                await this.Context.Channel.SendMessageAsync("Nie ma takiego wpisu byczq");
                Console.WriteLine("Couldn't read URL: Item not found");
            }
        }

        [Command("list")]
        [Alias("li", "lis")]
        public async Task ListKeysAsync()
        {
            Console.WriteLine($"Listing all keys");
            var keys = await this._dataAccess.FetchAttachmentKeys().ToListAsync();
            var resultMessage = keys.Aggregate((x, y) => $"{x} \n {y}");
            await this.Context.Channel.SendMessageAsync(resultMessage);
            Console.WriteLine("Succesfully listed all keys");
        }
    }
}