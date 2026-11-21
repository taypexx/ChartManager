using UnityEngine;

namespace ChartManager.Data.Charts;

internal class ChartAssets
{
    internal Chart Chart;
    internal Sprite Cover => Chart.Album.Cover;
    internal AudioClip Demo => Chart.Album.Demo;
    internal AudioClip Music => Chart.Album.Music;

    internal ChartAssets(Chart chart)
    {
        Chart = chart;
    }
}
