using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsyncTwitch;

namespace TwitchCameraMover
{
    class TwitchCommands
    {
        private readonly char commandCharacter = '!';
        private TwitchConnection twitch = null;

        public void handleCommandMessage(TwitchConnection twitch, TwitchMessage message)
        {
            string content = message.Content;
            this.twitch = twitch;

            if (String.IsNullOrEmpty(content))
            {
                Plugin.Log("Twitch message was empty or null", Plugin.LogLevel.Error);
                return;
            }

            // Check for command character
            Plugin.Log(content, Plugin.LogLevel.Debug);
            if (content.IndexOf(commandCharacter) != 0)
            {
                Plugin.Log("Twitch message didn't start with command character!", Plugin.LogLevel.Error);
                return;
            }

            runCommand(content.Substring(1).ToLower());
        }

        private void runCommand(string commandMessage)
        {
            Plugin.Log("Running commandMessage: " + commandMessage, Plugin.LogLevel.Debug);

            if (String.IsNullOrEmpty(commandMessage))
            {
                Plugin.Log("Command was null!", Plugin.LogLevel.Error);
                return;
            }

            string[] args = commandMessage.Split(null);
            string command = args[0];

            if (command.Equals("help"))
            {
                twitch.SendChatMessage(Consts.HELP_INFO);
                sendListOfValidCommands();
            }
            else if (command.Equals("move"))
            {
                if (args.Length < 5)
                {
                    Plugin.Log("Invalid args for command " + command, Plugin.LogLevel.Info);
                    twitch.SendChatMessage("USAGE: !move name x y z");
                    twitch.SendChatMessage("EXAMPLE: !move cam 2 0 1");
                    return;
                }

                string objectToMove = null;
                float xPos;
                float yPos;
                float zPos;

                // Example
                // !move cam 100 100 100
                try
                {
                    objectToMove = args[1].ToLower();
                    xPos = float.Parse(args[2]);
                    yPos = float.Parse(args[3]);
                    zPos = float.Parse(args[4]);
                }
                catch
                {
                    Plugin.Log("Error parsing commands! Format should be {command object float float float}: " + command, Plugin.LogLevel.Info);
                    twitch.SendChatMessage("USAGE: !move name x y z");
                    twitch.SendChatMessage("EXAMPLE: !move cam 2 0 1");
                    return;
                }

                if (objectToMove.Equals("cam") || objectToMove.Equals("camera"))
                {
                    CameraMover.Instance.moveCamera(xPos, yPos, zPos);
                }
                else
                {
                    Plugin.Log("Invalid object name for command: " + command, Plugin.LogLevel.Info);
                    sendListOfValidObjects();
                    return;
                }
            }
            else
            {
                Plugin.Log("Invalid command: " + command, Plugin.LogLevel.Info);
                sendListOfValidCommands();
                return;
            }
        }

        public void sendListOfValidObjects()
        {
            twitch.SendChatMessage("CURRENT OBJECTS: cam");
        }

        public void sendListOfValidCommands()
        {
            twitch.SendChatMessage("Current commands: !move");
        }
    }
}
