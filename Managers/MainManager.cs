using ChartManager.Data.Charts;
using ChartManager.Static;
using CustomAlbums.Managers;

namespace ChartManager.Managers
{
    internal static class MainManager
    {
        internal static Dictionary<string, Chart> Charts = new();

        internal static void RemoveChart(Chart chart)
        {
            var key = chart.Album.Uid;
            if (!Charts.ContainsKey(key)) return;
            Charts.Remove(key);
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
