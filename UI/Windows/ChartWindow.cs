using ChartManager.Data.Charts;
using ChartManager.Managers;
using ChartManager.Static;
using ChartManager.UI.Extensions;
using CustomAlbums.Managers;
using PopupLib.UI;
using PopupLib.UI.Components;
using PopupLib.UI.Windows.Abstract;
using PopupLib.UI.Windows.Interfaces;
using UnityEngine;

namespace ChartManager.UI.Windows;

internal class ChartWindow : Window
{
    private ForumObject AssetsButton;
    private ForumObject InfoButton;
    private ForumObject MapsButton;
    private ForumObject CinemaButton;
    private ForumObject SaveButton;
    private ForumObject DeleteButton;

    private readonly Confirmation SaveConfirmation;
    internal readonly Confirmation UnsavedConfirmation;
    private readonly Confirmation DeleteConfirmation;

    internal Chart CurrentChart { get; private set; }
    internal bool EditSaved { get; set; }

    private void OnSaveConfirm()
    {
        CurrentChart.SaveChanges();
        EditSaved = true;
        Show();
        PopupUtils.ShowInfo("Saved!");
    }

    private void OnUnsavedExit()
    {
        CurrentChart.RevertChanges();
        EditSaved = true;
        base.Close();
    }

    private void OnDeleteConfirm()
    {
        var prevMusicIndex = CurrentChart.MusicInfo.musicIndex - 1;
        if (prevMusicIndex < 0) prevMusicIndex = 2;

        var prevUid = $"{AlbumManager.Uid}-{prevMusicIndex}";

        CurrentChart.Delete();
        CurrentChart = null;

        // TODO: move to the closest chart instead of going back to the first
        base.Close();
        PopupUtils.ShowInfo("Deleted!");
        PnlStageExtension.Panel.SelectAllTagAndJumpToAssginIndex(prevUid);
    }

    internal void Show(Chart? chart = null)
    {
        if (chart != null && chart != CurrentChart)
        {
            CurrentChart = chart;
            EditSaved = true;
        }

        Title = new(CurrentChart.Info.Name + (EditSaved ? string.Empty : "*") );
        UpdateContent(CurrentChart.Info.ToString());
        UpdateBanner(CurrentChart.Info.BaseScene.ToString() + ".png");

        base.Show();
    }

    internal override void Close()
    {
        base.Close();
        if (!EditSaved) UnsavedConfirmation.Show();
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
        AssetsButton = AddButton("Assets");
        InfoButton = AddButton("Info", UIManager.InfoWindow);
        MapsButton = AddButton("Maps", UIManager.MapsWindow);
        CinemaButton = AddButton("Cinema");
        SaveButton = AddButton($"<color={Utils.ColorToHex(Color.white)}>< Save ></color>", SaveConfirmation);
        DeleteButton = AddButton($"<color={Utils.ColorToHex(Color.red)}>< Delete></color>", DeleteConfirmation);
    }

    internal ChartWindow() : base("Chart Editor", null, false)
    {
        SaveConfirmation = new("Saving", "Save the chart? This action will overwrite existing data.", OnSaveConfirm, Show);
        UnsavedConfirmation = new("Unsaved progress", "You have unsaved changes. Would you like to exit?", OnUnsavedExit, Show);
        DeleteConfirmation = new("Deleting", "This chart will be deleted forever. Would you like to proceed?", OnDeleteConfirm, Show);

        EditSaved = true;
    }
}
