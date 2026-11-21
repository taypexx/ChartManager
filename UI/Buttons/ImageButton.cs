using UnityEngine;
using UnityEngine.UI;

namespace ChartManager.UI.Buttons;

internal sealed class ImageButton : CustomButton
{
    internal GameObject ImageGameObject;
    internal RectTransform ImageRect;
    internal Image Image;

    internal void Update(Color imageColor, Color? baseColor = null)
    {
        Image.color = imageColor;
        if (baseColor != null) base.Update((Color)baseColor);
    }

    internal ImageButton(Sprite sprite, string name, Transform parent, Vector2 position, Vector2 size, Vector2 pivot, Action? onClick = null) :
    base(name, parent, position, size, pivot, onClick)
    {
        ImageGameObject = new GameObject("BtnImg");

        ImageRect = ImageGameObject.AddComponent<RectTransform>();
        ImageRect.parent = Rect.transform;
        ImageRect.anchoredPosition3D = Vector3.zero;
        ImageRect.localScale = Vector3.one;
        ImageRect.sizeDelta = Rect.sizeDelta;

        Image = ImageGameObject.AddComponent<Image>();
        Image.sprite = sprite;
    }
}
