using ChartManager.Static;
using UnityEngine;
using UnityEngine.UI;

namespace ChartManager.UI.Buttons;

internal sealed class TextButton : CustomButton
{
    internal GameObject TextGameObject;
    internal RectTransform TextRect;
    internal Text Text;

    internal void Update(string text, Color? baseColor = null)
    {
        Text.text = text;
        if (baseColor != null) base.Update((Color)baseColor);
    }

    internal TextButton(string name, Transform parent, Vector2 position, Vector2 size, Vector2 pivot, Action? onClick = null) :
    base(name, parent, position, size, pivot, onClick)
    {
        TextGameObject = new GameObject("BtnTxt");

        TextRect = TextGameObject.AddComponent<RectTransform>();
        TextRect.parent = Rect.transform;
        TextRect.anchoredPosition3D = Vector3.zero;
        TextRect.localScale = Vector3.one;
        TextRect.sizeDelta = Rect.sizeDelta;

        Text = TextGameObject.AddComponent<Text>();
        Text.alignment = TextAnchor.MiddleCenter;
        Text.text = name;
        Text.fontSize = 28;
        Text.horizontalOverflow = HorizontalWrapMode.Overflow;
        Text.font = Utils.NormalFont;
    }
}
