using ChartManager.Managers;
using Il2CppAssets.Scripts.PeroTools.Commons;
using Il2CppAssets.Scripts.PeroTools.Managers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ChartManager.UI.Buttons
{
    internal abstract class CustomButton
    {
        private static Sprite RoundedSquare = Addressables.LoadAssetAsync<Sprite>("SprRoundedsquare").WaitForCompletion();
        internal GameObject GameObject;
        internal RectTransform Rect;

        internal Image BaseImage;
        internal Button Button;

        internal void Update(Color baseColor)
        {
            BaseImage.color = baseColor;
        }

        internal Outline Outline;

        internal void CreateOutline(float size, Color color)
        {
            if (Outline != null) Component.Destroy(Outline);

            Outline = GameObject.AddComponent<Outline>();
            Outline.effectDistance = new(size, -size);
            Outline.effectColor = color;
        }

        protected CustomButton(string name, Transform parent, Vector2 position, Vector2 size, Vector2 pivot, Action? onClick = null)
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

            BaseImage = GameObject.AddComponent<Image>();
            BaseImage.sprite = RoundedSquare;
            BaseImage.type = Image.Type.Tiled;

            Button = GameObject.AddComponent<Button>();
            Button.onClick.AddListener(new Action(SoundManager.PlayClick));
            if (onClick != null) Button.onClick.AddListener((UnityAction)onClick);
        }
    }
}
