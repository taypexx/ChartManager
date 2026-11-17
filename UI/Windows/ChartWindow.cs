using ChartManager.Data;
using ChartManager.Managers;
using ChartManager.UI.Extensions;
using LocalizeLib;
using Multiplayer.Static;
using PopupLib.UI;
using PopupLib.UI.Components;
using PopupLib.UI.Windows.Abstract;
using PopupLib.UI.Windows.Interfaces;
using UnityEngine;
using UnityEngine.UI;

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

        internal Chart CurrentChart { get; private set; }
        internal bool EditSaved { get; private set; }

        private void SaveEdit()
        {
            MainManager.SaveChart(CurrentChart);
            EditSaved = true;
        }

        internal void Show(Chart? chart = null)
        {
            if (chart != null)
            {
                CurrentChart = chart;
                EditSaved = true;
            }

            Title = new(CurrentChart.Info.Name);

            SoundManager.PlayTroveBGM();
            base.Show();
        }

        internal override void Close()
        {
            if (EditSaved) 
                base.Close();
            else SaveConfirmation.Show();
        }

        internal override void OnShow(BaseWindow _)
        {
            BulletinExtension.ToggleCover(true, CurrentChart.Assets.Cover);
            base.OnShow(_);
        }

        internal override void OnClose(BaseWindow _)
        {
            SoundManager.RestoreMusic();
            BulletinExtension.ToggleCover(false);
            base.OnClose(_);
        }

        internal override void OnClick(IListWindow _, int objectIndex)
        {
            var button = Base.ForumObjects[objectIndex];
            
            if (button == SaveButton)
            {
                SaveEdit();
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
            SaveConfirmation = new("Unsaved progress", "You have unsaved changes. Would you like to exit?", base.Close, Show);
            EditSaved = true;
        }
    }
}
