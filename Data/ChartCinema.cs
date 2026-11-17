using ChartManager.Managers;
using ChartManager.Static;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ChartManager.Data
{
    internal class ChartCinema
    {
        private const string DefaultFileName = "video.mp4";
        private const float DefaultOpacity = 0.5f;

        internal Chart Chart;
        internal string TempPath => Path.Combine(Chart.GetTempFolder().FullName, "cinema.json");
        internal string FileName;
        internal float Opacity;
        internal byte[] Difficulties;

        internal JsonObject GetJson() => new()
        {
            ["file_name"] = FileName,
            ["opacity"] = Opacity,
            ["difficulties"] = Difficulties == null || Difficulties.Length == 0 ? null : new JsonArray(Difficulties.Select(d => JsonValue.Create(d)).ToArray()),
        };

        private void RefreshFromStream(Stream stream)
        {
            var json = JsonSerializer.Deserialize<JsonDocument>(stream);
            if (json == null)
            {
                Log.Error("Failed to parse cinema.json!");

                FileName = DefaultFileName;
                Opacity = DefaultOpacity;

                return;
            }

            if (json.RootElement.TryGetProperty("file_name", out var file_name))
            {
                var file_str = file_name.GetString();
                FileName = string.IsNullOrWhiteSpace(file_str) ? DefaultFileName : file_str;
            }
            else FileName = DefaultFileName;

            if (json.RootElement.TryGetProperty("opacity", out var opacity) && opacity.TryGetSingle(out var opacity_fl))
            {
                Opacity = opacity_fl;
            }
            else Opacity = DefaultOpacity;

            if (json.RootElement.TryGetProperty("difficulties", out var diffs) && diffs.ValueKind == JsonValueKind.Array)
            {
                foreach (var diff in diffs.EnumerateArray())
                {
                    if (!diff.TryGetByte(out var diff_b)) continue;
                    Difficulties.Append(diff_b);
                }
            }
        }

        internal void RefreshFromTemp()
        {
            try
            {
                if (!File.Exists(TempPath))
                {
                    throw new FileNotFoundException("File doesn't exist.");
                }

                // TODO: refresh here
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to refresh {Chart.Album.AlbumName} ChartCinema from the temp folder", ex);
            }
        }

        internal void RefreshFromAlbum()
        {
            try
            {
                var album = Chart.Album;
                if (!album.HasFile("cinema.json"))
                {
                    throw new FileNotFoundException("File doesn't exist.");
                }

                using var stream = album.OpenFileStreamIfPossible("cinema.json");
                RefreshFromStream(stream);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to refresh {Chart.Album.AlbumName} ChartCinema from the album", ex);
            }
        }

        internal void SaveToTemp()
        {
            try
            {
                var json = GetJson();
                using var stream = File.Create(TempPath);
                JsonSerializer.Serialize(stream, json, Utils.JsonOptions);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to save {Chart.Album.AlbumName} ChartCinema to the temp folder", ex);
            }
        }

        internal void SaveToAlbum()
        {
            try
            {
                var json = GetJson();
                SystemManager.WriteToAlbum(Chart.Album, "cinema.json", json.ToJsonString(Utils.JsonOptions));
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to save {Chart.Album.AlbumName} ChartCinema to the album", ex);
            }
        }

        internal ChartCinema(Chart chart)
        {
            Chart = chart;
            Difficulties = new byte[] { };

            RefreshFromAlbum();
        }
    }
}
