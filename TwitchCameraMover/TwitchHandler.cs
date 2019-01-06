using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsyncTwitch;

namespace TwitchCameraMover
{
    class TwitchHandler
    {
        private TwitchCommands twitchCommands = new TwitchCommands();
        private static bool twitchRegistered = false;

        public void init()
        {
            checkTwitchRegistration();
        }

        public void checkTwitchRegistration()
        {
            if (!twitchRegistered)
            {
                try
                {
                    TwitchConnection.Instance.StartConnection();
                    TwitchConnection.Instance.RegisterOnChatJoined(TwitchConnection_OnChatJoined);
                    TwitchConnection.Instance.RegisterOnMessageReceived(TwitchConnection_OnMessageReceived);
                    twitchRegistered = true;
                }
                catch (Exception ex)
                {
                    Plugin.Log(ex.ToString(), Plugin.LogLevel.Error);
                }
            }
        }

        private void TwitchConnection_OnMessageReceived(TwitchConnection arg1, TwitchMessage message)
        {
            Plugin.Log("Message Recieved, AsyncTwitch currently working", Plugin.LogLevel.Debug);
            twitchCommands.handleCommandMessage(arg1, message);
        }

        private void TwitchConnection_OnChatJoined(TwitchConnection arg1)
        {
            Plugin.Log("Message Recieved, AsyncTwitch currently working", Plugin.LogLevel.Debug);
            arg1.SendChatMessage("Custom UI plugin connected! Type !help for info!");
        }
    }
}
