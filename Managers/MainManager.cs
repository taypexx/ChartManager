using ChartManager.Data.Charts;
using ChartManager.Static;
using CustomAlbums.Managers;

namespace ChartManager.Managers
{
    internal static class MainManager
    {
        internal static Dictionary<string, Chart> Charts = new();

        internal static void SaveChart(Chart chart)
        {
            // NOTE: info must be pulled from the temp folder before saving so this save may not save anything at all

            // TODO: save assets

            chart.Info.SaveToAlbum();

            foreach (var (_,map) in chart.Maps)
            {
                map.SaveToAlbum();
            }

            if (chart.HasCinema) chart.Cinema.SaveToAlbum();
        }

        internal static void InitCharts()
        {
            foreach (var (albumName, album) in AlbumManager.LoadedAlbums)
            {
                try
                {
                    var info = new Chart(album);
                    Charts.Add(album.Uid, info);
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to initialize the info for {albumName}: {ex}");
                }
            }
        }
    }
}
