using ChartManager.Static;
using Il2CppAssets.Scripts.PeroTools.Commons;
using Il2CppAssets.Scripts.PeroTools.Managers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace ChartManager.UI
{
    internal sealed class CustomButton
    {
        private static Sprite RoundedSquare = Addressables.LoadAssetAsync<Sprite>("SprRoundedsquare").WaitForCompletion();
        internal GameObject GameObject;
        internal RectTransform Rect;

        internal Image Image;

        internal GameObject TextGameObject;
        internal RectTransform TextRect;
        internal Text Text;

        internal Button Button;

        internal void Update(string text, Color? baseColor = null, Color? textColor = null)
        {
            Text.text = text;
            if (baseColor != null) Image.color = (Color)baseColor;
            if (textColor != null) Text.color = (Color)textColor;
        }

        internal CustomButton(string name, Transform parent, Vector2 position, Vector2 size, Vector2 pivot, Action? onClick = null)
        {
            GameObject = new("Btn" + name);

            Rect = GameObject.AddComponent<RectTransform>();
            Rect.parent = parent;
            Rect.anchoredPosition3D = new(position.x, position.y, 0f);
            Rect.pivot = pivot;
            Rect.anchorMin = Rect.pivot;
            Rect.anchorMax = Rect.pivot;
            Rect.localScale = Vector3.one;
            Rect.sizeDelta = size;

            Image = GameObject.AddComponent<Image>();
            Image.sprite = RoundedSquare;
            Image.type = Image.Type.Tiled;

            GameObject.AddComponent<Button>();

            TextGameObject = new GameObject("BtnTxt");

            TextRect = TextGameObject.AddComponent<RectTransform>();
            TextRect.parent = Rect.transform;
            TextRect.anchoredPosition3D = Vector3.zero;
            TextRect.localScale = Vector3.one;
            TextRect.sizeDelta = Rect.sizeDelta;

            Text = TextGameObject.AddComponent<Text>();
            Text.alignment = TextAnchor.MiddleCenter;
            Text.text = "Button";
            Text.fontSize = 28;
            Text.horizontalOverflow = HorizontalWrapMode.Overflow;
            Text.font = Utils.NormalFont;

            Button = GameObject.AddComponent<Button>();
            Button.onClick.AddListener(new Action(() => Singleton<AudioManager>.instance.PlayButtonClickedSfx(ClickSfxType.Click)));
            if (onClick != null) Button.onClick.AddListener(onClick);
        }
    }
}
