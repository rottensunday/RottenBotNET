namespace NETDiscordBot.Services.DataAccess
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface IStreamDataAccessService
    {
        Task InitializeStreamEntry(string userId, string userName, bool isLonelyOnStart);

        Task FinishStream(string userId, bool isLonelyOnFinish);

        Task SwitchStateFromLonelyToNotLonely(string userId);
        
        Task SwitchStateFromNotLonelyToLonely(string userId);

        Task<int> FetchLonelyStreamingTime(string userId);

        Task<IEnumerable<StreamEntry>> FetchStreams(string username);
    }
}