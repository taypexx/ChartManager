using ChartManager.Data.Charts;
using ChartManager.Managers;
using ChartManager.Static;
using PopupLib.UI.Components;
using PopupLib.UI.Windows.Interfaces;

namespace ChartManager.UI.Windows
{
    internal class MapsWindow : Window
    {
        internal Chart CurrentChart => UIManager.ChartWindow.CurrentChart;
        private Dictionary<ForumObject, ChartMap> ButtonsMaps;

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
            UpdateBanner(UIManager.ChartWindow.Banner);

            base.Show();
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

        internal override void OnClick(IListWindow _, int objectIndex)
        {
            var button = Base.ForumObjects[objectIndex];
            
            if (ButtonsMaps.TryGetValue(button, out var map))
            {
                Utils.StartProcess(Path.Combine(map.Chart.GetTempFolder().FullName, map.BmsName));
                UIManager.ChartWindow.EditSaved = false;
            }

            base.OnClick(_, objectIndex);
        }

        internal MapsWindow() : base("Maps", null, true)
        {
            ButtonsMaps = new();
        }
    }
}
