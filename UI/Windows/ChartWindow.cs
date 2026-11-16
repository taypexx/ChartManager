using ChartManager.Data;
using ChartManager.Managers;
using PopupLib.UI.Components;
using PopupLib.UI.Windows;
using PopupLib.UI.Windows.Abstract;

namespace ChartManager.UI.Windows
{
    internal class ChartWindow : Window
    {
        private static string WebsiteChartMessage = "";
        private ForumObject NameButton;
        private ForumObject ArtistButton;

        internal Chart CurrentChart { get; private set; }

        private static void OnCloneConfirmed() 
        {

        }

        internal void Show(Chart? chart = null)
        {
            if (chart != null)
            {
                CurrentChart = chart;

                if (chart.OnWebsite)
                {
                    
                    return;
                }

                SoundManager.PlayAchievementsBGM();
            }

            base.Show();
        }

        internal void Close(BaseWindow _)
        {
            SoundManager.RestoreMusic();
            base.OnClose(_);
        }

        internal ChartWindow() : base("Chart Editor", null, false)
        {
            NameButton = AddButton("Name");
            ArtistButton = AddButton("Artist");
        }
    }
}
