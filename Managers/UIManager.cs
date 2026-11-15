using ChartManager.UI;
using UnityEngine;

namespace ChartManager.Managers
{
    internal static class UIManager
    {
        internal static CustomButton CreateButton;
        internal static CustomButton ManageButton;

        internal static void Load()
        {
            BulletinExtension.Create();

            // Buttons
            CreateButton = new(
                "Create",
                GameObject.Find("UI/Standerd/PnlStage/StageUi/Info").transform,
                new Vector2(-164f, -32f),
                new Vector2(100f, 100f),
                new Vector2(1f, 1f)
            );

            ManageButton = new(
                "Manage",
                GameObject.Find("UI/Standerd/PnlStage/StageUi/Info").transform,
                new Vector2(-32f, -32f),
                new Vector2(100f, 100f),
                new Vector2(1f, 1f)
            );
        }
    }
}
