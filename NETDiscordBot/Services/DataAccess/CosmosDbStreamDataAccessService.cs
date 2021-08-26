namespace NETDiscordBot.Services.DataAccess
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Azure.Cosmos;
    using Microsoft.Extensions.Configuration;
    using Models.CosmosDb;

    public class CosmosDbStreamDataAccessService : IStreamDataAccessService
    {
        private string EndpointUrl => _configuration["EndpointUrl"];
        private string AuthorizationKey => _configuration["AuthorizationKey"];
        private string DatabaseId => _configuration["DatabaseId"];
        private string ContainerId => _configuration["StreamsContainerId"];

        private readonly CosmosContainer _streamsContainer;
        private readonly IConfiguration _configuration;

        public CosmosDbStreamDataAccessService(
            IConfiguration configuration)
        {
            this._configuration = configuration;
            var client = new CosmosClient(EndpointUrl, AuthorizationKey);
            this._streamsContainer = client.GetContainer(DatabaseId, ContainerId);
        }

        public async Task InitializeStreamEntry(string userId, bool isLonelyOnStart)
        {
            var now = DateTime.Now;
            var entry = new CosmosDbStreamEntry()
            {
                Id = Guid.NewGuid().ToString(),
                StreamStart = now,
                UserId = userId,
                LonelyStreamTimestamp = isLonelyOnStart ? now : null
            };

            await this._streamsContainer.CreateItemAsync(entry);
        }

        public async Task FinishStream(string userId, bool isLonelyOnFinish)
        {
            var streamEntry = await FetchLatestStreamByUser(userId);

            if (streamEntry is not null)
            {
                var now = DateTime.Now;
                streamEntry.StreamEnd = now;

                if (isLonelyOnFinish && streamEntry.LonelyStreamTimestamp is not null)
                {
                    streamEntry.TotalLonelyStreamTime ??= 0;
                    streamEntry.TotalLonelyStreamTime += (now - streamEntry.LonelyStreamTimestamp).Value.Seconds;
                }

                await this._streamsContainer.UpsertItemAsync(streamEntry);
            }
        }

        public async Task SwitchStateFromLonelyToNotLonely(string userId)
        {
            var streamEntry = await FetchLatestStreamByUser(userId);

            if (streamEntry is not null)
            {
                if (streamEntry.LonelyStreamTimestamp is not null)
                {
                    streamEntry.TotalLonelyStreamTime ??= 0;
                    streamEntry.TotalLonelyStreamTime += (DateTime.Now - streamEntry.LonelyStreamTimestamp).Value.Seconds;
                    streamEntry.LonelyStreamTimestamp = null;
                }
                
                await this._streamsContainer.UpsertItemAsync(streamEntry);
            }
        }

        public async Task SwitchStateFromNotLonelyToLonely(string userId)
        {
            var streamEntry = await FetchLatestStreamByUser(userId);

            if (streamEntry is not null)
            {
                streamEntry.LonelyStreamTimestamp = DateTime.Now;
                
                await this._streamsContainer.UpsertItemAsync(streamEntry);
            }
        }

        public async Task<int> FetchLonelyStreamingTime(string userId)
        {
            var times = await _streamsContainer
                .GetItemQueryIterator<CosmosDbStreamEntry>($"select * from c where c.userId = \"{userId}\"")
                .ToListAsync();

            return times.Sum(x => x.TotalLonelyStreamTime ?? 0);
        }

        private async Task<CosmosDbStreamEntry> FetchLatestStreamByUser(string userId)
            => await _streamsContainer
                .GetItemQueryIterator<CosmosDbStreamEntry>(
                    $"select top 1 * from c where c.userId=\"{userId}\" order by c.streamStart desc")
                .FirstOrDefaultAsync();
    }
}