namespace NETDiscordBot.Services.DataAccess
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Azure.Cosmos;
    using Microsoft.Extensions.Configuration;
    using Models;
    using Models.CosmosDb;

    public class CosmosDbAttachmentDataAccessService : IAttachmentDataAccessService
    {
        private string EndpointUrl => _configuration["EndpointUrl"];
        private string AuthorizationKey => _configuration["AuthorizationKey"];
        private string DatabaseId => _configuration["DatabaseId"];
        private string ContainerId => _configuration["UrlsContainerId"];

        private readonly CosmosContainer _attachmentsContainer;
        private readonly IConfiguration _configuration;

        public CosmosDbAttachmentDataAccessService(
            IConfiguration configuration)
        {
            this._configuration = configuration;
            var client = new CosmosClient(EndpointUrl, AuthorizationKey);
            this._attachmentsContainer = client.GetContainer(DatabaseId, ContainerId);
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