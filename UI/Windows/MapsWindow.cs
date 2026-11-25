using ChartManager.Data.Charts;
using ChartManager.Managers;
using ChartManager.Static;
using ChartManager.UI.Extensions;
using PopupLib.UI;
using PopupLib.UI.Components;
using PopupLib.UI.Windows.Abstract;
using PopupLib.UI.Windows.Interfaces;

namespace ChartManager.UI.Windows;

internal class MapsWindow : Window
{
    private ChartWindow ChartWindow => UIManager.ChartWindow;
    internal Chart CurrentChart => ChartWindow.CurrentChart;
    private readonly Dictionary<ForumObject, ChartMap> ButtonsMaps;

    internal override void Show()
    {
        RemoveButtons();
        CreateButtons();

        var content = string.Empty;
        foreach (var (diff, map) in CurrentChart.Maps)
        {
            content += map.ToString() + "\n";
        }

        UpdateContent(content);
        UpdateBanner(ChartWindow.Banner);

        base.Show();
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

    internal override void OnClick(IListWindow _, int objectIndex)
    {
        var button = Base.ForumObjects[objectIndex];

        if (ButtonsMaps.TryGetValue(button, out var map))
        {
            PopupUtils.ShowInfo($"Opening {map.BmsName}...");
            Utils.StartProcess(Path.Combine(map.Chart.GetTempFolder().FullName, map.BmsName));
            ChartWindow.EditSaved = false;
            return;
        }

        base.OnClick(_, objectIndex);
    }

    internal override void CreateButtons()
    {
        if (CurrentChart == null) return;

        foreach (var (diff, map) in CurrentChart.Maps)
        {
            var button = AddButton($"{Utils.DifficultyNames[diff]} {map.Level}★");
            ButtonsMaps.Add(button, map);
        }
    }

    internal MapsWindow() : base("Maps", null, true)
    {
        ButtonsMaps = new();
    }
}
