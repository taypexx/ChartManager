using System.Reflection;
using ChartManager.Managers;
using ChartManager.Static;
using ChartManager.UI;
using MelonLoader;

namespace ChartManager
{
    public class Main : MelonMod
    {
        public const string ModName = "ChartManager";
        public const string ModDescription = "A mod that lets you find, delete and create charts through muse dash.";
        public const string Version = "0.1.0";
        public const string Author = "taypexx";

        internal static Assembly CurrentAssembly = Assembly.GetExecutingAssembly();
        internal static string CurrentScene { get; private set; }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            CurrentScene = sceneName;

            switch (CurrentScene)
            {
                case "UISystem_PC":
                    UIManager.Load();
                    break;
                case "GameMain":
                    break;
            }
        }

        public override void OnLateInitializeMelon()
        {
            AssetManager.Init();
        }

        public override void OnInitializeMelon()
        {
            Log.Info("ChartManager initialized successfully.");
        }
    }
}
