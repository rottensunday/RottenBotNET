namespace NETDiscordBot.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Azure.Cosmos;
    using Models;
    using Models.CosmosDb;

    public class CosmosDbDataAccessService : IDataAccessService
    {
        private const string EndpointUrl = "";
        private const string AuthorizationKey = "";
        private const string DatabaseId = "";
        private const string ContainerId = "";

        private readonly CosmosClient _client;
        private readonly CosmosContainer _attachmentsContainer;

        public CosmosDbDataAccessService()
        {
            this._client = new CosmosClient(EndpointUrl, AuthorizationKey);
            this._attachmentsContainer = _client.GetContainer(DatabaseId, ContainerId);
        }
        
        public async Task SaveAttachmentEntry(AttachmentEntry entry)
        {
            await this._attachmentsContainer.UpsertItemAsync(
                CosmosDbAttachmentEntry.BuildFromEntry(entry), 
                new PartitionKey(entry.Key));
        }

        public async Task<AttachmentEntry> LoadAttachmentEntry(string key)
        {
             var response = await _attachmentsContainer.ReadItemAsync<CosmosDbAttachmentEntry>(
                 key, 
                 new PartitionKey(key));

             return new AttachmentEntry(response);
        }

        public IAsyncEnumerable<string> FetchAttachmentKeys()
        {
            return _attachmentsContainer
                    .GetItemQueryIterator<KeyEntry>("select c.key from c")
                    .Select(item => item.Key);
        }
    }
}