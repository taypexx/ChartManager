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
        private ForumObject AssetsButton;
        private ForumObject InfoButton;
        private ForumObject MapsButton;
        private ForumObject CinemaButton;
        private ForumObject SaveButton;
        private ForumObject DeleteButton;

        private Confirmation SaveConfirmation;
        private Confirmation UnsavedConfirmation;
        private Confirmation DeleteConfirmation;

        internal Chart CurrentChart { get; private set; }
        internal bool EditSaved { get; set; }

        private void OnSaveConfirm()
        {
            CurrentChart.Save();
            EditSaved = true;
            Show();
        }

        private void OnDeleteConfirm()
        {
            CurrentChart.Delete();
            CurrentChart = null;

            // TODO: move to the closest chart instead of going back to the first
            base.Close();
        }

        internal void Show(Chart? chart = null)
        {
            if (chart != null)
            {
                CurrentChart = chart;
                EditSaved = true;
            }

            Title = new(CurrentChart.Info.Name);
            UpdateContent(CurrentChart.Info.ToString());
            UpdateBanner(CurrentChart.Info.BaseScene.ToString() + ".png");

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
            else if (button == DeleteButton)
            {
                DeleteConfirmation.Show();
            }

            base.OnClick(_, objectIndex);
        }

        internal override void CreateButtons()
        {
            AssetsButton = AddButton("Assets");
            InfoButton = AddButton("Info");
            MapsButton = AddButton("Maps", UIManager.MapsWindow);
            CinemaButton = AddButton("Cinema");
            SaveButton = AddButton("< Save >");
            DeleteButton = AddButton("< Delete>");
        }

        internal ChartWindow() : base("Chart Editor", null, false)
        {
            SaveConfirmation = new("Saving", "Save the chart? This action will overwrite existing data.", OnSaveConfirm, Show);
            UnsavedConfirmation = new("Unsaved progress", "You have unsaved changes. Would you like to exit?", base.Close, Show);
            DeleteConfirmation = new("Deleting", "This chart will be deleted forever. Would you like to proceed?", OnDeleteConfirm, Show);

            EditSaved = true;
        }
    }
}
