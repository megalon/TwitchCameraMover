using System;
using IllusionPlugin;
using UnityEngine.SceneManagement;

namespace TwitchCameraMover
{
    public class Plugin : IPlugin
    {
        public string Name => "TwitchCameraMover";
        public string Version => "0.0.1";
        public static Plugin Instance = null;

        private static bool debug = true;
        private TwitchHandler twitchHandler = new TwitchHandler();

        public void OnApplicationStart()
        {
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            CameraMover instance = CameraMover.Instance;
            if (instance != null)
            {
                instance.SceneManagerOnActiveSceneChanged(arg0, arg1);
            }

            twitchHandler.checkTwitchRegistration();
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name == "Menu")
            {
                CameraMover.OnLoad();
            }
        }

        public void OnApplicationQuit()
        {
            if (Plugin.Instance == null)
            {
                Plugin.Instance = this;
            }
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }

        public void OnLevelWasLoaded(int level)
        {

        }

        public void OnLevelWasInitialized(int level)
        {
        }

        public void OnUpdate()
        {
        }

        public void OnFixedUpdate()
        {
        }

        public enum LogLevel { Debug, Info, Error };

        public static void Log(string text, LogLevel logLevel)
        {
            if (logLevel == LogLevel.Debug && debug)
            {
                Console.WriteLine("[TwitchCameraMover] " + text);
            } else
            {
                Console.WriteLine("[TwitchCameraMover] " + text);
            }
        }
    }
}
