using ChartManager.Data.Charts;
using ChartManager.Managers;
using ChartManager.Static;
using ChartManager.UI.Extensions;
using PopupLib.UI;
using PopupLib.UI.Components;
using PopupLib.UI.Windows.Abstract;
using PopupLib.UI.Windows.Interfaces;

namespace ChartManager.UI.Windows.InfoWindows;

internal class SceneWindow : Window
{
    private InfoWindow InfoWindow => UIManager.InfoWindow;
    private Chart CurrentChart => UIManager.ChartWindow.CurrentChart;
    private ChartInfo CurrentInfo => InfoWindow.CurrentInfo;
    private Dictionary<ForumObject, Scene> ButtonsScenes;

    private void OnSceneSelected(Scene scene)
    {
        if (CurrentInfo != null)
        {
            CurrentInfo.BaseScene = scene;
            CurrentInfo.SaveToTemp();

            PopupUtils.ShowInfo($"Set to {Utils.StringFromScene(scene)}");
        }
        Close();
        InfoWindow.Show();
    }

    internal override void OnClick(IListWindow _, int objectIndex)
    {
        if (CurrentInfo != null)
        {
            var button = Base.ForumObjects[objectIndex];
            foreach (var (button_, scene) in ButtonsScenes)
            {
                if (button_ == button)
                {
                    OnSceneSelected(scene);
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
        foreach (var scene_ in Enum.GetValues(typeof(Scene)))
        {
            var scene = (Scene)scene_;
            if (scene == Scene.Unknown) continue;

            var button = AddButton(Utils.StringFromScene(scene));
            ButtonsScenes.Add(button, scene);
        }
    }

    internal SceneWindow() : base("Scene", null, true)
    {
        ButtonsScenes = new();
    }
}
