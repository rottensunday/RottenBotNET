namespace NETDiscordBot.Models
{
    using System.Text.Json.Serialization;
    using CosmosDb;

    public class AttachmentEntry
    {
        public string Key { get; init; }
        
        public string Url { get; init; }

        public AttachmentEntry(string key, string url)
        {
            this.Key = key;
            this.Url = url;
        }
        
        public AttachmentEntry(CosmosDbAttachmentEntry entry)
        {
            this.Key = entry.Key;
            this.Url = entry.Url;
        }
    }
}