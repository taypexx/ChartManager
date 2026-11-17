using ChartManager.Managers;
using ChartManager.Static;
using CustomAlbums.Data;
using Il2CppAssets.Scripts.Database;
using System.Net.Http.Json;
using System.Text.Json;

namespace ChartManager.Data
{
    internal class Chart
    {
        internal Album Album;
        internal MusicInfo MusicInfo => GlobalDataBase.dbMusicTag.GetMusicInfoFromAll(Album.Uid);

        internal ChartInfo Info;
        internal ChartAssets Assets;
        internal Dictionary<int, ChartMap> Maps;
        internal ChartCinema? Cinema;
        internal bool HasCinema => Cinema != null;

        private bool? OnWebsite;
        private string? WebsiteId;
        private bool? IsRanked;

        private DirectoryInfo TempFolder;

        internal DirectoryInfo GetTempFolder()
        {
            if (TempFolder == null)
            {
                TempFolder = SystemManager.CopyAlbumFiles(Album);
            }
            return TempFolder;
        }

        internal bool IsOnWebsite()
        {
            return OnWebsite == null ? false : (bool)OnWebsite;
        }

        internal async Task<bool> IsOnWebsiteAsync(bool forceUpdate = true)
        {
            if (forceUpdate || OnWebsite == null)
            {
                await UpdateWebsiteInfo();
            }
            return (bool)OnWebsite;
        }

        internal async Task UpdateWebsiteInfo()
        {
            using var response = await Client.GetAsync("https://api.mdmc.moe/v3/sheets/" + Album.Sheets[2].Md5, false);

            // We check for the 404 specifically, because the server might be down or anything
            if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                OnWebsite = response.IsSuccessStatusCode;
                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadFromJsonAsync<Dictionary<string, JsonElement>>();
                    var chartData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(body["chart"]);
                    WebsiteId = chartData["id"].GetString();
                    IsRanked = chartData["ranked"].GetBoolean();
                }
            }
            else OnWebsite = false;
        }

        internal Chart(Album album)
        {
            Album = album;
            Info = new(this);
            Assets = new(this);

            Maps = new();
            foreach (var (diff, sheet) in album.Sheets)
            {
                Maps.Add(diff, new(this, diff));
            }

            if (album.HasFile("cinema.json"))
            {
                Cinema = new(this);
            }
        }
    }
}
