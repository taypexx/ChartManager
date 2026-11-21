using ChartManager.Managers;
using ChartManager.Static;
using ChartManager.UI.Windows;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ChartManager.UI.Extensions
{
    internal static class BulletinExtension
    {
        private const string BulletinPath = "UI/Forward/Tips/PnlBulletinNew";
        internal static GameObject Bulletin => GameObject.Find(BulletinPath);
        internal static GameObject ImageBase => GameObject.Find($"{BulletinPath}/ImgBase");
        internal static Image BannerImage => GameObject.Find($"{BulletinPath}/ImgBase/ScrollView/Viewport/Content/Image").GetComponent<Image>();

        internal static Window CurrentWindow { get; private set; }
        internal static Window PreviousWindow { get; private set; }

        internal static Text Title { get; private set; }
        internal static Button BackButton { get; private set; }
        internal static Button ExitButton { get; private set; }
        internal static Image CoverImage { get; private set; }

        internal static void SetWindow(Window window, string title, bool hasBackButton)
        {
            PreviousWindow = CurrentWindow;
            CurrentWindow = window;
            Title.text = title;
            Toggle(true, hasBackButton);
        }

        internal static void CloseWindow()
        {
            CurrentWindow = null;
            Toggle(false);
        }

        internal static void ToggleCover(bool state, Sprite cover = null)
        {
            CoverImage.gameObject.SetActive(state);
            if (cover != null)
            {
                CoverImage.sprite = cover;
            }
        }

        internal static void Toggle(bool state, bool enableBackButton = true)
        {
            Title.gameObject.SetActive(state);
            BackButton.gameObject.SetActive(state && enableBackButton);
            ExitButton.gameObject.SetActive(state);
        }

        private static Button CreateTopButton(Sprite sprite, Vector2 anchoredPosition, UnityAction action)
        {
            var btnGo = new GameObject("Btn");

            var btnRect = btnGo.AddComponent<RectTransform>();
            btnRect.parent = ImageBase.transform;
            btnRect.localScale = new(1f, 1f, 1f);
            btnRect.sizeDelta = new(70f, 70f);
            btnRect.pivot = new(0f, 1f);
            btnRect.anchorMin = btnRect.pivot;
            btnRect.anchorMax = btnRect.pivot;
            btnRect.anchoredPosition3D = anchoredPosition;

            var btnBase = btnGo.AddComponent<Image>();
            btnBase.color = new(0.298f, 0.1176f, 0.5216f, 1f);
            btnBase.sprite = ImageBase.GetComponent<Image>().sprite;

            var btnIconGo = new GameObject("Icon");

            var btnIconRect = btnIconGo.AddComponent<RectTransform>();
            btnIconRect.parent = btnBase.transform;
            btnIconRect.localScale = btnRect.localScale;
            btnIconRect.sizeDelta = new(50f, 50f);
            btnIconRect.anchoredPosition3D = Vector3.zero;

            var btnIcon = btnIconGo.AddComponent<Image>();
            btnIcon.sprite = sprite;
            btnIcon.color = new(0.737f, 0.494f, 1f, 1f);

            var btn = btnGo.AddComponent<Button>();
            btn.onClick.AddListener(action);

            return btn;
        }

        internal static void Create()
        {
            var windowTitleGo = GameObject.Instantiate(Bulletin.transform.Find("TxtTittle").gameObject, Bulletin.transform);
            windowTitleGo.name = "WindowTitle";
            windowTitleGo.SetActive(false);
            Component.Destroy(windowTitleGo.GetComponent<Il2CppAssets.Scripts.PeroTools.GeneralLocalization.Localization>());

            var titleRect = windowTitleGo.GetComponent<RectTransform>();
            titleRect.pivot = new(0.5f, 1f);
            titleRect.anchorMin = titleRect.pivot;
            titleRect.anchorMax = titleRect.pivot;
            titleRect.anchoredPosition = new(0f, -170f);

            Title = windowTitleGo.GetComponent<Text>();
            Title.font = Utils.NormalFont;
            Title.text = "Window";

            BackButton = CreateTopButton(AssetManager.GetImageAsset("Icons.Back.png").Sprite, new(40f, 40f), new Action(() =>
            {
                if (PreviousWindow == null) return;
                if (CurrentWindow != null) CurrentWindow.Close();
                PreviousWindow.Show();
            }));

            ExitButton = CreateTopButton(AssetManager.GetImageAsset("Icons.Cross.png").Sprite, new(1090f, 40f), new Action(() =>
            {
                if (CurrentWindow == null) return;
                CurrentWindow.Close();
            }));

            var coverGameObject = new GameObject("Cover");
            var coverRect = coverGameObject.AddComponent<RectTransform>();
            coverRect.SetParent(ImageBase.transform.Find("ScrollView/Viewport/Content/Image"));

            var coverPivot = new Vector2(0f, 0.5f);
            coverRect.pivot = coverPivot;
            coverRect.anchorMin = coverPivot;
            coverRect.anchorMax = coverPivot;
            coverRect.localScale = Vector3.one;
            coverRect.sizeDelta = new(200f, 200f);
            coverRect.anchoredPosition3D = new(20f, 0f, 0f);

            CoverImage = coverGameObject.AddComponent<Image>();
        }
    }
}