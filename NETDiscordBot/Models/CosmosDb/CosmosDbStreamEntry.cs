namespace NETDiscordBot.Models.CosmosDb
{
    using System;
    using System.Text.Json.Serialization;

    public class CosmosDbStreamEntry
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("userId")]
        public string UserId { get; set; }
        
        [JsonPropertyName("userName")]
        public string UserName { get; set; }
        
        [JsonPropertyName("streamStart")]
        public DateTime StreamStart { get; set; }
        
        [JsonPropertyName("streamEnd")]
        public DateTime? StreamEnd { get; set; }
        
        [JsonPropertyName("lonelyStreamTimestamp")]
        public DateTime? LonelyStreamTimestamp { get; set; }
        
        [JsonPropertyName("lonelyStreamTime")]
        public int? TotalLonelyStreamTime { get; set; }

        public static CosmosDbStreamEntry BuildFromEntry(StreamEntry entry)
        {
            return new CosmosDbStreamEntry()
            {
                Id = new Guid().ToString(),
                StreamEnd = entry.StreamEnd,
                StreamStart = entry.StreamStart,
                UserId = entry.UserId,
                UserName = entry.UserName,
                LonelyStreamTimestamp = entry.LonelyStreamTimestamp,
                TotalLonelyStreamTime = entry.TotalLonelyStreamTime
            };
        }
    }
}