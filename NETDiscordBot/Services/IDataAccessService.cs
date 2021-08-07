namespace NETDiscordBot.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface IDataAccessService
    {
        Task SaveAttachmentEntry(AttachmentEntry entry);
        Task<AttachmentEntry> LoadAttachmentEntry(string key);
        IAsyncEnumerable<string> FetchAttachmentKeys();
    }
}