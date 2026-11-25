using ChartManager.Data.Charts;
using ChartManager.Managers;
using PopupLib.UI;
using PopupLib.UI.Components;
using PopupLib.UI.Windows;
using PopupLib.UI.Windows.Interfaces;

namespace ChartManager.UI.Windows.InfoWindows;

internal class TagsWindow : Window
{
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

    private void OnTagAdded(PopupLib.UI.Windows.Abstract.BaseWindow _)
    {
        if (AddTagInput.Result != null && CurrentInfo != null)
        {
            var tag = AddTagInput.Result;
            CurrentInfo.Tags = CurrentInfo.Tags.Append(tag).ToArray();
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
            PopupUtils.ShowInfo($"Removed \"{tag}\" tag");
        }
        Refresh();
    }

    internal override void OnClick(IListWindow _, int objectIndex)
    {
        var button = Base.ForumObjects[objectIndex];

        foreach (var (button_, tagIndex) in ButtonsTagIndexes)
        {
            if (button == button_)
            {
                OnTagRemoved(tagIndex);
                break;
            }
        }

        base.OnClick(_, objectIndex);
    }

    internal override void CreateButtons()
    {
        AddTagButton = AddButton("Add a tag", AddTagInput);
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
        AddTagInput = new() { AutoReset = true };
        AddTagInput.OnCompletion += OnTagAdded;
    }
}
