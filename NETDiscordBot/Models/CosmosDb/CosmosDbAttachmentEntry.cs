namespace NETDiscordBot.Models.CosmosDb
{
    using System;
    using System.Text.Json.Serialization;

    public class CosmosDbAttachmentEntry
    {
        [JsonPropertyName("id")]
        public string Id { get; init; }
        
        [JsonPropertyName("key")]
        public string Key { get; init; }
        
        [JsonPropertyName("url")]
        public string Url { get; init; }

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