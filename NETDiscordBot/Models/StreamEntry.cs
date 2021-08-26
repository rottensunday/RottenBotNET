namespace NETDiscordBot.Models
{
    using System;
    using CosmosDb;

    public class StreamEntry
    {
        public string UserId { get; init; }
        
        public DateTime StreamStart { get; init; }
        
        public DateTime? StreamEnd { get; init; }
        
        public DateTime? LonelyStreamTimestamp { get; init; }
        
        public int? TotalLonelyStreamTime { get; init; }

        public StreamEntry(CosmosDbStreamEntry entry)
        {
            this.StreamEnd = entry.StreamEnd;
            this.StreamStart = entry.StreamStart;
            this.UserId = entry.UserId;
            this.LonelyStreamTimestamp = entry.LonelyStreamTimestamp;
            this.TotalLonelyStreamTime = entry.TotalLonelyStreamTime;
        }
    }
}