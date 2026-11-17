using UnityEngine;
using Il2CppAssets.Scripts.UI.Panels.PnlMusicTag;
using ChartManager.UI.Extensions;
using ChartManager.UI.Windows;
using Il2CppAssets.Scripts.UI.Panels;

namespace ChartManager.Managers
{
    internal static class UIManager
    {
        internal static PnlMusicTag PnlMusicTag => GameObject.Find("UI/Forward/PnlMusicTag").GetComponent<PnlMusicTag>();
        internal static PnlStage PnlStage => GameObject.Find("UI/Standerd/PnlStage").GetComponent<PnlStage>();

        internal static ChartWindow ChartWindow;
        internal static MapsWindow MapsWindow;

        /// <summary>
        /// Loads the UI when the main UI scene loads.
        /// </summary>
        internal static void BuildUI()
        {
            BulletinExtension.Create();
            PnlStageExtension.CreateButtons();
        }

        internal static void BuildWindows()
        {
            ChartWindow = new();
            MapsWindow = new();

            ChartWindow.CreateButtons();
            MapsWindow.CreateButtons();
        }
    }
}
