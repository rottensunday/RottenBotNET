namespace NETDiscordBot.Modules
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using Models;
    using Preconditions;
    using Services.DataAccess;

    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        private readonly IAttachmentDataAccessService _attachmentDataAccess;
        private readonly IStreamDataAccessService _streamDataAccess;

        public PublicModule(
            IAttachmentDataAccessService attachmentDataAccess,
            IStreamDataAccessService streamDataAccess)
        {
            this._attachmentDataAccess = attachmentDataAccess;
            this._streamDataAccess = streamDataAccess;
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
        public async Task SaveAttachmentAsync([Remainder] string key)
        {
            var attachmentUrl = this.Context.Message.Attachments.First().Url;
            Console.WriteLine($"Saving attachment with key: {key}, url: {attachmentUrl}");
            
            await this._attachmentDataAccess.SaveAttachmentEntry(new AttachmentEntry(key, attachmentUrl));
            await this.Context.Channel.SendMessageAsync("Dodano wpis mordo");
            Console.WriteLine("Succesfully saved attachment");
        }

        [Command("load")]
        [Alias("l", "lo", "loa")]
        public async Task LoadAttachmentAsync([Remainder] string key)
        {
            Console.WriteLine($"Loading attachment with key: {key}");

            try
            {
                var result = await this._attachmentDataAccess.LoadAttachmentEntry(key);
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
            var keys = await this._attachmentDataAccess.FetchAttachmentKeys().ToListAsync();
            var resultMessage = keys.Aggregate((x, y) => $"{x} \n {y}");
            await this.Context.Channel.SendMessageAsync(resultMessage);
            Console.WriteLine("Succesfully listed all keys");
        }

        [Command("lonelystreamer")]
        [Alias("ls", "lonely", "streamer")]
        public async Task FetchLonelyStreamingTime(IUser user)
        {
            Console.WriteLine($"Fetching lonely streaming time of user: {user.Username} with ID: {user.Id}");
            var timeInSeconds = await this._streamDataAccess.FetchLonelyStreamingTime(user.Id.ToString());
            var hours = Convert.ToSingle(timeInSeconds) / 3600;
            await this.Context.Channel.SendMessageAsync(
                $"{user.Username} streamował samotnie {hours:F3} godzin! Gratulacje.");
            Console.WriteLine($"Succesfully fetched lonely streaming time of user {user.Username}");
        }
    }
}