namespace NETDiscordBot.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Discord.WebSocket;

    enum StreamEvent
    {
        StreamStarted = 0,
        StreamStartedLonely = 1,
        StreamEnded = 2,
        StreamEndedLonely = 3,
        NotImportant = 100,
        
    }
    
    public class StreamingMonitorService
    {
        private readonly IStreamDataAccessService _streamDataAccessService;

        public StreamingMonitorService(
            IStreamDataAccessService streamDataAccessService)
        {
            this._streamDataAccessService = streamDataAccessService;
        }

        public async Task<bool> Handle(
            SocketUser user,
            SocketVoiceState previousState,
            SocketVoiceState currentState)
        {
            await this.HandleCurrentUser(user, previousState, currentState);

            var userToHandleFromPreviousChannel = 
                previousState.VoiceChannel?.Users.FirstOrDefault(x => x.IsStreaming && x.Id != user.Id);
            var userToHandleFromCurrentChannel = 
                currentState.VoiceChannel?.Users.FirstOrDefault(x => x.IsStreaming && x.Id != user.Id);
            var isSameChannel = previousState.VoiceChannel?.Id == currentState.VoiceChannel?.Id;

            if (!isSameChannel)
            {
                if (previousState.VoiceChannel?.Users.Count == 1 && userToHandleFromPreviousChannel is not null)
                {
                    await this.HandleSwitchStateFromNotLonelyToLonely(userToHandleFromPreviousChannel.Id.ToString());
                }

                if (currentState.VoiceChannel?.Users.Count == 2 && userToHandleFromCurrentChannel is not null)
                {
                    await this.HandleSwitchStateFromLonelyToNotLonely(userToHandleFromCurrentChannel.Id.ToString());
                }
            }
            
            return true;
        }

        private async Task<bool> HandleCurrentUser(
            SocketUser user,
            SocketVoiceState previousState,
            SocketVoiceState currentState)
            => DetermineStreamEvent(user, previousState, currentState) switch
            {
                StreamEvent.StreamStarted => await this.HandleStreamStarted(user.Id.ToString(), false),
                StreamEvent.StreamStartedLonely => await this.HandleStreamStarted(user.Id.ToString(), true),
                StreamEvent.StreamEnded => await this.HandleStreamFinished(user.Id.ToString(), false),
                StreamEvent.StreamEndedLonely => await this.HandleStreamFinished(user.Id.ToString(), true),
                _ => false
            };
        
        private StreamEvent DetermineStreamEvent(
            SocketUser user,
            SocketVoiceState previousState,
            SocketVoiceState currentState)
        {
            if (!previousState.IsStreaming && currentState.IsStreaming)
            {
                if (currentState.VoiceChannel.Users.Count == 1)
                {
                    return StreamEvent.StreamStartedLonely;
                }
                else
                {
                    return StreamEvent.StreamStarted;
                }
            }

            if (previousState.IsStreaming && (!currentState.IsStreaming || currentState.VoiceChannel is null))
            {
                var isSameVoiceChannel = previousState.VoiceChannel == currentState.VoiceChannel;
                if ((isSameVoiceChannel && previousState.VoiceChannel?.Users.Count == 1) 
                    || (!isSameVoiceChannel && previousState.VoiceChannel?.Users.Count == 0))
                {
                    return StreamEvent.StreamEndedLonely;
                }
                else
                {
                    return StreamEvent.StreamEnded;
                }
            }

            return StreamEvent.NotImportant;
        }

        private async Task<bool> HandleStreamStarted(
            string userId,
            bool isLonely)
        {
            await this._streamDataAccessService.InitializeStreamEntry(userId, isLonely);

            return true;
        }

        private async Task<bool> HandleStreamFinished(
            string userId,
            bool isLonely)
        {
            await this._streamDataAccessService.FinishStream(userId, isLonely);

            return true;
        }

        private async Task<bool> HandleSwitchStateFromLonelyToNotLonely(string userId)
        {
            await this._streamDataAccessService.SwitchStateFromLonelyToNotLonely(userId);

            return true;
        }

        private async Task<bool> HandleSwitchStateFromNotLonelyToLonely(string userId)
        {
            await this._streamDataAccessService.SwitchStateFromNotLonelyToLonely(userId);

            return true;
        }
    }
}