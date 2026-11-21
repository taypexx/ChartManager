using ChartManager.Managers;
using ChartManager.Static;
using ChartManager.UI.Buttons;
using CustomAlbums.Managers;
using HarmonyLib;
using Il2CppAssets.Scripts.Database;
using Il2CppAssets.Scripts.UI.Panels;
using PopupLib.UI;
using UnityEngine;

namespace ChartManager.UI.Extensions;

internal static class PnlStageExtension
{
    internal static PnlStage Panel => UIManager.PnlStage;
    internal static GameObject PnlInfo => GameObject.Find("UI/Standerd/PnlStage/StageUi/Info");

    internal static ImageButton CreateButton;
    internal static ImageButton ManageButton;
    internal static ImageButton EditButton;

    internal static void CreateButtons()
    {
        // Create
        CreateButton = new(
            AssetManager.GetImageAsset("Icons.Plus.png").Sprite, "Create",
            PnlInfo.transform,
            new Vector2(-148f, -64f),
            new Vector2(80f, 80f),
            new Vector2(1f, 1f),
            () =>
            {
                PopupUtils.ShowInfo("Coming soon...");
            }
        );
        CreateButton.Update(Palette.Yellow3, Palette.Yellow1);
        CreateButton.CreateOutline(4, Palette.Yellow2);

        // Manage
        ManageButton = new(
            AssetManager.GetImageAsset("Icons.List.png").Sprite, "Manage",
            PnlInfo.transform,
            new Vector2(-32f, -64f),
            new Vector2(80f, 80f),
            new Vector2(1f, 1f),
            () =>
            {
                PnlMusicTagExtension.ShowModified();
            }
        );
        ManageButton.Update(Palette.Yellow3, Palette.Yellow1);
        ManageButton.CreateOutline(4, Palette.Yellow2);

        // Edit
        EditButton = new(
            AssetManager.GetImageAsset("Icons.Pen.png").Sprite, "Edit",
            PnlInfo.transform,
            new Vector2(350f, -100f),
            new Vector2(64f, 64f),
            new Vector2(0.5f, 1f),
            () =>
            {
                var musicInfo = GlobalDataBase.dbMusicTag.CurMusicInfo();
                if (musicInfo.albumIndex != AlbumManager.Uid) return;

                var chart = MainManager.Charts[musicInfo.uid];
                UIManager.ChartWindow.Show(chart);
            }
        );
        EditButton.Update(Palette.Purple3, Palette.Purple1);
        EditButton.CreateOutline(4, Palette.Purple2);

        // Move the vanilla copy button
        var btnCopyRect = PnlInfo.transform.Find("BtnCopy").GetComponent<RectTransform>();
        btnCopyRect.anchoredPosition = new(-1300f, 125f);
    }
}

[HarmonyPatch(typeof(PnlStage), nameof(PnlStage.Update))]
internal static class PnlStageUpdatePatch
{
    private static void Postfix(PnlStage __instance)
    {
        if (PnlStageExtension.EditButton == null) return;

        // Disable edit button if the selected cell is random/non-custom
        PnlStageExtension.EditButton.GameObject.SetActive(GlobalDataBase.dbMusicTag.CurMusicInfo().albumIndex == AlbumManager.Uid);
    }
}
