using ChartManager.Data.Charts;
using ChartManager.Managers;
using ChartManager.Static;
using ChartManager.UI.Extensions;
using ChartManager.UI.Windows.InfoWindows;
using PopupLib.UI;
using PopupLib.UI.Components;
using PopupLib.UI.Windows;
using PopupLib.UI.Windows.Abstract;
using PopupLib.UI.Windows.Interfaces;
using UnityEngine;

namespace ChartManager.UI.Windows;

internal class InfoWindow : Window
{
    private ChartWindow ChartWindow => UIManager.ChartWindow;
    private Chart CurrentChart => ChartWindow.CurrentChart;
    internal ChartInfo CurrentInfo => CurrentChart?.Info;

    private ForumObject NameButton;
    private ForumObject NameRomanizedButton;
    private ForumObject AuthorButton;
    private ForumObject BPMButton;
    private ForumObject SceneButton;
    private ForumObject TagsButton;
    private ForumObject MapsButton;
    private ForumObject HiddenButton;
    private ForumObject SceneEggButton;
    private ForumObject EditManuallyButton;

    private InputWindow NameInput;
    private InputWindow NameRomanizedInput;
    private InputWindow AuthorInput;
    private InputWindow BPMInput;

    private SceneWindow SceneWindow;
    private TagsWindow TagsWindow;
    private SceneEggWindow SceneEggWindow;

    internal override void Show()
    {
        if (CurrentInfo != null)
        {
            UpdateContent(CurrentInfo.ToString());
            UpdateBanner(ChartWindow.Banner);
        }

        base.Show();
    }

    private void OnNameInput(BaseWindow _)
    {
        if (NameInput.Result != null && CurrentInfo != null)
        {
            CurrentInfo.Name = NameInput.Result;
            CurrentInfo.SaveToTemp();
        }
        Show();
    }

    private void OnNameRomanizedInput(BaseWindow _)
    {
        if (NameRomanizedInput.Result != null && CurrentInfo != null)
        {
            CurrentInfo.NameRomanized = NameRomanizedInput.Result;
            CurrentInfo.SaveToTemp();
        }
        Show();
    }

    private void OnAuthorInput(BaseWindow _)
    {
        if (AuthorInput.Result != null && CurrentInfo != null)
        {
            CurrentInfo.Author = AuthorInput.Result;
            CurrentInfo.SaveToTemp();
        }
        Show();
    }

    private void OnBPMInput(BaseWindow _)
    {
        if (BPMInput.Result != null && CurrentInfo != null)
        {
            CurrentInfo.BPM = BPMInput.Result;
            CurrentInfo.SaveToTemp();
        }
        Show();
    }

    internal override void OnClick(IListWindow _, int objectIndex)
    {
        if (CurrentInfo != null)
        {
            var button = Base.ForumObjects[objectIndex];

            if (button == EditManuallyButton)
            {
                PopupUtils.ShowInfo("Opening info.json...");
                Utils.StartProcess(Path.Combine(CurrentChart.GetTempFolder().FullName, "info.json"));
                ChartWindow.EditSaved = false;
                Refresh();
                return;
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
        NameButton = AddButton("Name", NameInput);
        NameRomanizedButton = AddButton("Name Romanized", NameRomanizedInput);
        AuthorButton = AddButton("Author", AuthorInput);
        BPMButton = AddButton("BPM", BPMInput);
        SceneButton = AddButton("Scene", SceneWindow);
        TagsButton = AddButton("Tags", TagsWindow);
        MapsButton = AddButton("Manage Maps");
        HiddenButton = AddButton("Hidden Settings");
        SceneEggButton = AddButton("Scene Egg");

        EditManuallyButton = AddButton($"<color={Utils.ColorToHex(Color.white)}>< Edit manually ></color>");
    }

    internal InfoWindow() : base("Info", null, true)
    {
        NameInput = new() { AutoReset = true };
        NameRomanizedInput = new() { AutoReset = true };
        AuthorInput = new() { AutoReset = true };
        BPMInput = new() { AutoReset = true };

        NameInput.OnCompletion += OnNameInput;
        NameRomanizedInput.OnCompletion += OnNameRomanizedInput;
        AuthorInput.OnCompletion += OnAuthorInput;
        BPMInput.OnCompletion += OnBPMInput;

        SceneWindow = new();
        SceneWindow.CreateButtons();

        TagsWindow = new();

        SceneEggWindow = new();
        SceneEggWindow.CreateButtons();
    }
}
