namespace NETDiscordBot.Services.DataAccess
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface IAttachmentDataAccessService
    {
        Task SaveAttachmentEntry(AttachmentEntry entry);
        
        Task<AttachmentEntry> LoadAttachmentEntry(string key);
        
        IAsyncEnumerable<string> FetchAttachmentKeys();
    }
}