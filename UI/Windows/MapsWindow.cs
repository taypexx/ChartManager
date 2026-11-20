using ChartManager.Data.Charts;
using ChartManager.Managers;
using ChartManager.Static;
using PopupLib.UI.Components;
using PopupLib.UI.Windows.Interfaces;

namespace ChartManager.UI.Windows
{
    internal class MapsWindow : Window
    {
        private static readonly Dictionary<int, string> DifficultyNames = new()
        {
            [1] = "Easy",
            [2] = "Hard",
            [3] = "Master",
            [4] = "Hidden",
            [5] = "Touhou"
        };
        internal Chart CurrentChart => UIManager.ChartWindow.CurrentChart;
        private Dictionary<ForumObject, ChartMap> ButtonsMaps;

        internal override void Show()
        {
            RemoveButtons();
            CreateButtons();

            base.Show();
        }

        internal override void CreateButtons()
        {
            if (CurrentChart == null) return;

            foreach (var (diff, map) in CurrentChart.Maps)
            {
                var button = AddButton(DifficultyNames[diff]);
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
