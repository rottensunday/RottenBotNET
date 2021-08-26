namespace NETDiscordBot.Services.DataAccess
{
    using System.Threading.Tasks;

    public interface IStreamDataAccessService
    {
        Task InitializeStreamEntry(string userId, bool isLonelyOnStart);

        Task FinishStream(string userId, bool isLonelyOnFinish);

        Task SwitchStateFromLonelyToNotLonely(string userId);
        
        Task SwitchStateFromNotLonelyToLonely(string userId);

        Task<int> FetchLonelyStreamingTime(string userId);
    }
}