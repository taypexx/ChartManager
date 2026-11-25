using ChartManager.Data.Charts;
using ChartManager.Managers;
using ChartManager.UI.Extensions;
using CustomAlbums.Data;
using PopupLib.UI;
using PopupLib.UI.Components;
using PopupLib.UI.Windows.Abstract;
using PopupLib.UI.Windows.Interfaces;

namespace ChartManager.UI.Windows.InfoWindows;

internal class SceneEggWindow : Window
{
    private InfoWindow InfoWindow => UIManager.InfoWindow;
    private Chart CurrentChart => UIManager.ChartWindow.CurrentChart;
    private ChartInfo CurrentInfo => InfoWindow.CurrentInfo;
    private Dictionary<ForumObject, SceneEgg.SceneEggs> ButtonsSceneEggs;

    private void OnSceneEggSelected(SceneEgg.SceneEggs sceneEgg)
    {
        if (CurrentInfo != null)
        {
            CurrentInfo.SceneEgg = sceneEgg;
            CurrentInfo.SaveToTemp();

            PopupUtils.ShowInfo($"Scene Egg set to {sceneEgg.ToString()}");
        }

        Close();
        InfoWindow.Show();
    }

    internal override void OnClick(IListWindow _, int objectIndex)
    {
        if (CurrentInfo != null)
        {
            var button = Base.ForumObjects[objectIndex];
            foreach (var (button_, sceneEgg) in ButtonsSceneEggs)
            {
                if (button_ == button)
                {
                    OnSceneEggSelected(sceneEgg);
                    return;
                }
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
        foreach (var sceneEgg_ in Enum.GetValues(typeof(SceneEgg.SceneEggs)))
        {
            var sceneEgg = (SceneEgg.SceneEggs)sceneEgg_;
            var button = AddButton(sceneEgg.ToString());
            ButtonsSceneEggs.Add(button, sceneEgg);
        }
    }

    internal SceneEggWindow() : base("Scene Egg", null, true)
    {
        ButtonsSceneEggs = new();
    }
}
