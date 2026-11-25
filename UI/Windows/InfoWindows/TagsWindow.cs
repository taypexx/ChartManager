using ChartManager.Data.Charts;
using ChartManager.Managers;
using ChartManager.Static;
using ChartManager.UI.Extensions;
using PopupLib.UI;
using PopupLib.UI.Components;
using PopupLib.UI.Windows;
using PopupLib.UI.Windows.Abstract;
using PopupLib.UI.Windows.Interfaces;
using UnityEngine;

namespace ChartManager.UI.Windows.InfoWindows;

internal class TagsWindow : Window
{
    private Chart CurrentChart => UIManager.ChartWindow.CurrentChart;
    private ChartInfo CurrentInfo => UIManager.InfoWindow.CurrentInfo;
    private Dictionary<ForumObject, int> ButtonsTagIndexes;

    private ForumObject AddTagButton;
    private InputWindow AddTagInput;

    internal override void Show()
    {
        RemoveButtons();
        CreateButtons();

        base.Show();
    }

    private void OnTagAdded(BaseWindow _)
    {
        if (!string.IsNullOrWhiteSpace(AddTagInput.Result) && CurrentInfo != null)
        {
            var tag = AddTagInput.Result;

            CurrentInfo.Tags = CurrentInfo.Tags.Append(tag).ToArray();
            CurrentInfo.SaveToTemp();

            PopupUtils.ShowInfo($"Added \"{tag}\" tag");
        }
        Show();
    }

    private void OnTagRemoved(int tagIndex)
    {
        if (CurrentInfo != null)
        {
            var tag = CurrentInfo.Tags[tagIndex];

            CurrentInfo.Tags = CurrentInfo.Tags.Where(t => t != tag).ToArray();
            CurrentInfo.SaveToTemp();

            PopupUtils.ShowInfo($"Removed \"{tag}\" tag");
        }
        Close();
        Refresh();
    }

    internal override void OnClick(IListWindow _, int objectIndex)
    {
        if (CurrentInfo != null)
        {
            var button = Base.ForumObjects[objectIndex];
            foreach (var (button_, tagIndex) in ButtonsTagIndexes)
            {
                if (button_ == button)
                {
                    OnTagRemoved(tagIndex);
                    return;
                }
            }
        }
        base.OnClick(_, objectIndex);
    }

    internal override void OnShow(BaseWindow _)
    {
        BulletinExtension.ToggleCover(true, CurrentChart.Assets.Cover);
        base.OnShow(_);
    }

    internal override void OnClose(BaseWindow _)
    {
        BulletinExtension.ToggleCover(false);
        base.OnClose(_);
    }

    internal override void CreateButtons()
    {
        AddTagButton = AddButton($"<color={Utils.ColorToHex(Color.white)}>< Add a tag ></color>", AddTagInput);
        if (CurrentInfo == null) return;

        ButtonsTagIndexes.Clear();
        for (int i = 0; i < CurrentInfo.Tags.Length; i++)
        {
            var tag = CurrentInfo.Tags[i];
            var button = AddButton(tag);
            ButtonsTagIndexes.Add(button, i);
        }
    }

    internal TagsWindow() : base("Tags", null, true)
    {
        ButtonsTagIndexes = new();

        AddTagInput = new() { AutoReset = true };
        AddTagInput.OnCompletion += OnTagAdded;
    }
}
