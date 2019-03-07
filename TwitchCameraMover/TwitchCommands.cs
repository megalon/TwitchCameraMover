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
            var config = CameraMover.Instance.Config;


            if (!message.Author.IsBroadcaster)
            {
                if (config.CommandsRestricted)
                {
                    if (message.Author.IsVIP)
                    {
                        if (!config.AllowForVips) return;
                    }
                    else if (message.Author.IsMod)
                    {
                        if (!config.AllowForMods) return;
                    }
                    else if (message.Author.IsSubscriber)
                    {
                        if (!config.AllowForSubs) return;
                    }
                    else
                    {
                        return;
                    }
                }
            }

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
            else if (command.Equals("cam"))
            {
                if (args.Length < 2)
                {
                    Plugin.Log("Invalid sub arg: " + command, Plugin.LogLevel.Info);
                    sendListOfValidCommands();
                    return;
                }

                var action = args[1].ToLower();
                if (action.Equals("move"))
                {

                    if (args.Length < 5)
                    {
                        Plugin.Log("Invalid args for command " + command, Plugin.LogLevel.Info);
                        twitch.SendChatMessage("USAGE: !cam move x y z");
                        twitch.SendChatMessage("EXAMPLE: !cam move 2 0 1");
                        return;
                    }

                    string objectToMove = null;
                    float xPos;
                    float yPos;
                    float zPos;

                    // Example
                    // !cam move 100 100 100
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

                    CameraMover.Instance.moveCamera(xPos, yPos, zPos);

                }
                else if (action.Equals("slide"))
                {
                    float xPos;
                    float yPos;
                    float zPos;
                    float speed;
                    string moveType;

                    try
                    {
                        xPos = float.Parse(args[2]);
                        yPos = float.Parse(args[3]);
                        zPos = float.Parse(args[4]);
                        speed = float.Parse(args[5]);
                        moveType = args[6] == null ? null : args[6].ToLower();
                    }
                    catch
                    {
                        Plugin.Log("Error parsing commands! Format should be {command object float float float}: " + command, Plugin.LogLevel.Info);
                        twitch.SendChatMessage("USAGE: !cam slide x y z dur [motionType]");
                        twitch.SendChatMessage("EXAMPLE: !cam slide 2 0 1 5 ease");
                        return;
                    }

                    CameraMover.Instance.slideCamera(xPos, yPos, zPos, speed, moveType);
                }
                else if (action.Equals("queue"))
                {
                    var qargs = commandMessage.Substring(Math.Max(0, "!cam move ".Length));
                    var slideActions = qargs.Split(',');
                    foreach (var slideAction in slideActions)
                    {
                        float xPos;
                        float yPos;
                        float zPos;
                        float speed;
                        string moveType;
                        var slideArgs = slideAction.Trim().ToLower().Split(' ');

                        try
                        {
                            xPos = float.Parse(slideArgs[0]);
                            yPos = float.Parse(slideArgs[1]);
                            zPos = float.Parse(slideArgs[2]);
                            speed = float.Parse(slideArgs[3]);
                            moveType = slideArgs[4] == null ? null : slideArgs[4].ToLower();
                        }
                        catch
                        {
                            Plugin.Log("Error parsing commands! Format should be {command object float float float}: " + command, Plugin.LogLevel.Info);
                            twitch.SendChatMessage("USAGE: !cam queue x y z dur [motionType],x y z dur [motionType]");
                            twitch.SendChatMessage("EXAMPLE: !cam queue 2 0 1 5 ease,-2 0 1 5 bezier");
                            return;
                        }

                        CameraMover.Instance.slideCamera(xPos, yPos, zPos, speed, moveType);
                    }
                }
                else if (action.Equals("stop"))
                {
                    CameraMover.Instance.stop();
                    twitch.SendChatMessage("Camera motion stopped!");
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
            twitch.SendChatMessage("Current commands: !cam move, !cam slide");
        }
    }
}
