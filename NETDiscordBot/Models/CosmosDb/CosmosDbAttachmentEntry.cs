namespace NETDiscordBot.Models.CosmosDb
{
    using System.Text.Json.Serialization;

    public class CosmosDbAttachmentEntry
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("key")]
        public string Key { get; set; }
        
        [JsonPropertyName("url")]
        public string Url { get; set; }

        public static CosmosDbAttachmentEntry BuildFromEntry(AttachmentEntry entry)
        {
            var cosmosDbEntry = new CosmosDbAttachmentEntry()
                {
                    Id = entry.Key,
                    Key = entry.Key,
                    Url = entry.Url
                };
            
            return cosmosDbEntry;
        }
    }
}