using ChartManager.Data;

namespace ChartManager.UI.Windows
{
    internal class MapsWindow : Window
    {
        private static Dictionary<int, string> DifficultyNames = new()
        {
            [1] = "Easy",
            [2] = "Hard",
            [3] = "Master",
            [4] = "Hidden",
            [5] = "Touhou"
        };

        internal Chart CurrentChart { get; private set; }

        internal void Show(Chart? chart = null)
        {
            if (chart != null)
            {
                CurrentChart = chart;
            }

            RemoveButtons();
            CreateButtons();

            base.Show();
        }

        internal override void CreateButtons()
        {
            if (CurrentChart == null) return;

            foreach (var diff in CurrentChart.Maps.Keys)
            {
                AddButton(DifficultyNames[diff]);
            }
        }

        internal MapsWindow() : base("Maps", "MapsWindow.png", true)
        {

        }
    }
}
