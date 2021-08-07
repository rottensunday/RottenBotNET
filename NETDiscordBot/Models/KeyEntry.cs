namespace NETDiscordBot.Models
{
    using System.Text.Json.Serialization;

    public class KeyEntry
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }
    }
}