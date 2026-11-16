using ChartManager.Managers;
using CustomAlbums.Managers;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.PeroTools.Nice.Components;
using Il2CppAssets.Scripts.UI.Panels.PnlMusicTag;
using UnityEngine;

namespace ChartManager.UI.Extensions
{
    internal static class PnlMusicTagExtension
    {
        internal static bool Modified = false;

        private static PnlMusicTag Panel => UIManager.PnlMusicTag;
        private static GameObject BtnOk => Panel.transform.Find("BaseArea/BottomArea/BtnOk").gameObject;

        private static PnlMusicTagItem PnlMusicTagItem => (PnlMusicTagItem)Panel.TabItems[0];
        private static GameObject TagGroup1 => PnlMusicTagItem.m_TagGroup1.gameObject;
        private static GameObject TagGroup2 => PnlMusicTagItem.m_TagGroup2.gameObject;
        private static string[] Tag1Keep = new string[] { "tag-custom-albums", "hide", "collections" };

        private static void SetTagGroupActive(GameObject tagGroup, bool active)
        {
            for (int i = 0; i < tagGroup.transform.childCount; i++)
            {
                var tag = tagGroup.transform.GetChild(i);
                tag.gameObject.SetActive(active);
            }
        }

        internal static void Toggle(bool state)
        {
            BtnOk.active = !state;

            if (state)
            {
                for (int i = 0; i < TagGroup1.transform.childCount; i++)
                {
                    var tag = TagGroup1.transform.GetChild(i);
                    tag.gameObject.SetActive(Tag1Keep.Contains(tag.name));
                }
                SetTagGroupActive(TagGroup2, false);
                Panel.SetTabIndex(0);
            }
            else
            {
                SetTagGroupActive(TagGroup1, true);
                SetTagGroupActive(TagGroup2, true);
            }
        }

        internal static void ShowModified()
        {
            Modified = true;
            Panel.Show();
        }
    }

    [HarmonyPatch(typeof(PnlMusicTag), nameof(PnlMusicTag.OnCellClicked))]
    internal static class PnlMusicTagSelectPatch
    {
        private static bool Prefix(MusicButtonCell cell)
        {
            // Continue if not modified
            if (!PnlMusicTagExtension.Modified) return true;

            // Select the chart if modified
            UIManager.PnlMusicTag.Close();

            if (cell.musicInfo.albumIndex == AlbumManager.Uid)
            {
                var chart = MainManager.Charts[cell.musicInfo.uid];
                UIManager.ChartWindow.Show(chart);
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(PnlMusicTag), nameof(PnlMusicTag.Show))]
    internal static class PnlMusicTagShowPatch
    {
        private static void Postfix()
        {
            PnlMusicTagExtension.Toggle(PnlMusicTagExtension.Modified);
        }
    }

    [HarmonyPatch(typeof(PnlMusicTag), nameof(PnlMusicTag.Close))]
    internal static class PnlMusicTagClosePatch
    {
        private static void Postfix()
        {
            PnlMusicTagExtension.Modified = false;
        }
    }
}
