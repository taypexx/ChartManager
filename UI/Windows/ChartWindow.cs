using ChartManager.Data.Charts;
using ChartManager.Managers;
using ChartManager.UI.Extensions;
using PopupLib.UI.Components;
using PopupLib.UI.Windows.Abstract;
using PopupLib.UI.Windows.Interfaces;

namespace ChartManager.UI.Windows
{
    internal class ChartWindow : Window
    {
        private ForumObject InfoButton;
        private ForumObject AssetsButton;
        private ForumObject MapsButton;
        private ForumObject CinemaButton;
        private ForumObject SaveButton;

        private Confirmation SaveConfirmation;
        private Confirmation UnsavedConfirmation;

        internal Chart CurrentChart { get; private set; }
        internal bool EditSaved { get; private set; }

        private void SaveEdit()
        {
            MainManager.SaveChart(CurrentChart);
            EditSaved = true;
            Show();
        }

        internal void Show(Chart? chart = null)
        {
            if (chart != null)
            {
                CurrentChart = chart;
                EditSaved = true;
            }

            Title = new(CurrentChart.Info.Name);

            base.Show();
        }

        internal override void Close()
        {
            if (EditSaved) 
                base.Close();
            else UnsavedConfirmation.Show();
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
            
            if (button == SaveButton)
            {
                SaveConfirmation.Show();
            }

            base.OnClick(_, objectIndex);
        }

        internal override void CreateButtons()
        {
            InfoButton = AddButton("Info");
            AssetsButton = AddButton("Assets");
            MapsButton = AddButton("Maps", UIManager.MapsWindow);
            CinemaButton = AddButton("Cinema");
            SaveButton = AddButton("< Save >", this);
        }

        internal ChartWindow() : base("Chart Editor", "ChartWindow.png", false)
        {
            SaveConfirmation = new("Saving", "Save the chart? This action will overwrite existing data.", SaveEdit, Show);
            UnsavedConfirmation = new("Unsaved progress", "You have unsaved changes. Would you like to exit?", base.Close, Show);
            EditSaved = true;
        }
    }
}
