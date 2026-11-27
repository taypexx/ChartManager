using ChartManager.Data.Charts;
using ChartManager.Static;
using CustomAlbums.Data;
using CustomAlbums.Managers;
using Il2CppSystem.Threading.Tasks;

namespace ChartManager.Managers;

internal static class MainManager
{
    internal static Dictionary<string, Chart> Charts = new();

    internal static Chart GetChart(string uid)
    {
        if (Charts.ContainsKey(uid))
        {
            return Charts[uid];
        }

        var album = AlbumManager.GetByUid(uid);
        return InitChart(album);
    }

    internal static void RemoveChart(Chart chart)
    {
        var key = chart.Album.Uid;
        if (!Charts.ContainsKey(key)) return;
        _ = Charts.Remove(key);
        chart.Dispose();
    }

    private static Chart InitChart(Album album)
    {
        try
        {
            var chart = new Chart(album);
            Charts.Add(album.Uid, chart);

            return chart;
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to initialize the info for {album.AlbumName}: {ex}");
            return null;
        }
    }

    internal static void InitCharts()
    {
        foreach (var (_, album) in AlbumManager.LoadedAlbums)
        {
            InitChart(album);
        }
    }
}
