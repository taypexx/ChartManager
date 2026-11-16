using System.Reflection;
using ChartManager.Managers;
using ChartManager.Static;
using MelonLoader;

namespace ChartManager
{
    public class Main : MelonMod
    {
        public const string ModName = "ChartManager";
        public const string ModDescription = "A mod that lets you find, delete and create charts through muse dash.";
        public const string Version = "1.0.0";
        public const string Author = "taypexx";
        private static readonly string[] Dependencies = { "CustomAlbums", "LocalizeLib", "PopupLib" };

        internal static Assembly CurrentAssembly = Assembly.GetExecutingAssembly();
        internal static string CurrentScene { get; private set; }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            CurrentScene = sceneName;

            switch (CurrentScene)
            {
                case "UISystem_PC":
                    UIManager.BuildUI();
                    break;
                case "GameMain":
                    break;
            }
        }

        public override void OnLateInitializeMelon()
        {
            AssetManager.Init();
            UIManager.BuildWindows();

            // Creating infos after all albums were loaded
            MainManager.InitCharts();
        }

        public override void OnInitializeMelon()
        {
            // Checking dependencies
            foreach (var dependencyName in Dependencies)
            {
                if (!RegisteredMelons.Any(m => m.Info.Name == dependencyName))
                {
                    Log.Error($"{dependencyName} dependency is missing!");
                }
            }

            SystemManager.CreateMainFolder();
        }

        public override void OnDeinitializeMelon()
        {
            SystemManager.Cleanup();
        }
    }
}
