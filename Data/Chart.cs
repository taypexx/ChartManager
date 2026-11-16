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

        internal bool OnWebsite;
        internal string? WebsiteId;
        internal bool IsRanked;

        private DirectoryInfo TempFolder;

        internal DirectoryInfo GetTempFolder()
        {
            if (TempFolder == null)
            {
                TempFolder = SystemManager.CopyAlbumFiles(Album);
            }
            return TempFolder;
        }

        internal async Task UpdateWebsiteInfo()
        {
            using var response = await Client.GetAsync("https://api.mdmc.moe/v3/sheets/" + Album.Sheets[2].Md5);

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
        }

        internal Chart(Album album)
        {
            Album = album;
            OnWebsite = false;
            IsRanked = false;
            _ = UpdateWebsiteInfo();
        }
    }
}
