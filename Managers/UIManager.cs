using ChartManager.UI;
using ChartManager.UI.Buttons;
using ChartManager.Static;
using UnityEngine;

namespace ChartManager.Managers
{
    internal static class UIManager
    {
        internal static ImageButton CreateButton;
        internal static ImageButton ManageButton;

        /// <summary>
        /// Creates the PnlStage buttons.
        /// </summary>
        private static void BuildButtons()
        {
            CreateButton = new(
                AssetManager.GetImageAsset("Icons.Pen.png").Sprite, "Create",
                GameObject.Find("UI/Standerd/PnlStage/StageUi/Info").transform,
                new Vector2(-148f, -64f),
                new Vector2(80f, 80f),
                new Vector2(1f, 1f),
                () =>
                {

                }
            );
            CreateButton.Update(Palette.Purple1, Palette.Purple2);
            CreateButton.CreateOutline(4, Palette.Purple1);

            ManageButton = new(
                AssetManager.GetImageAsset("Icons.List.png").Sprite, "Manage",
                GameObject.Find("UI/Standerd/PnlStage/StageUi/Info").transform,
                new Vector2(-32f, -64f),
                new Vector2(80f, 80f),
                new Vector2(1f, 1f),
                () =>
                {

                }
            );
            ManageButton.Update(Palette.Purple1, Palette.Purple2);
            ManageButton.CreateOutline(4, Palette.Purple1);
        }

        /// <summary>
        /// Loads the UI when the main UI scene loads.
        /// </summary>
        internal static void Load()
        {
            BulletinExtension.Create();
            BuildButtons();
        }
    }
}
