using ChartManager.Managers;
using ChartManager.Static;
using CustomAlbums.Data;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ChartManager.Data
{
    internal class ChartInfo
    {
        internal Chart Chart;
        internal string TempPath => Path.Combine(Chart.GetTempFolder().FullName, "info.json");
        internal string Name;
        internal string? NameRomanized;
        internal string Author;
        internal string BPM;
        internal string[] Tags;
        internal string LevelDesigner;
        internal Scene BaseScene;
        internal SceneEgg.SceneEggs SceneEgg;
        internal HiddenUnlockMethod HiddenUnlockMethod;
        internal HiddenUnlockType HiddenUnlockType;
        internal string HiddenUnlockMessage;

        internal JsonObject GetJson() => new()
        {
            ["name"] = Name,
            ["name_romanized"] = NameRomanized,
            ["author"] = Author,
            ["bpm"] = BPM,
            ["scene"] = $"scene_{((byte)BaseScene).ToString("D2")}",
            ["searchTags"] = new JsonArray(Tags.Select(t => JsonValue.Create(t)).ToArray()),
            ["levelDesigner"] = LevelDesigner,
            ["levelDesigner1"] = Chart.Maps.TryGetValue(1, out var map1) ? map1.LevelDesigner : null,
            ["levelDesigner2"] = Chart.Maps.TryGetValue(2, out var map2) ? map2.LevelDesigner : null,
            ["levelDesigner3"] = Chart.Maps.TryGetValue(3, out var map3) ? map3.LevelDesigner : null,
            ["levelDesigner4"] = Chart.Maps.TryGetValue(4, out var map4) ? map4.LevelDesigner : null,
            ["levelDesigner5"] = Chart.Maps.TryGetValue(5, out var map5) ? map5.LevelDesigner : null,
            ["difficulty1"] = map1 != null ? map1.Level : "0",
            ["difficulty2"] = map2 != null ? map2.Level : "0",
            ["difficulty3"] = map3 != null ? map3.Level : "0",
            ["difficulty4"] = map4 != null ? map4.Level : "0",
            ["difficulty5"] = map5 != null ? map5.Level : "0",
            ["hideBmsMode"] = HiddenUnlockMethod.ToString(),
            ["hideBmsDifficulty"] = ((sbyte)HiddenUnlockType).ToString(),
            ["hideBmsMessage"] = HiddenUnlockMessage
        };

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
                Log.Error($"Failed to refresh {Chart.Album.AlbumName} ChartInfo from the temp folder");
            }
        }

        internal void RefreshFromAlbum()
        {
            try
            {
                var album = Chart.Album;
                if (!album.HasFile("info.json"))
                {
                    throw new FileNotFoundException("File doesn't exist.");
                }

                var info = album.Info;

                Name = info.Name;
                NameRomanized = string.IsNullOrWhiteSpace(info.NameRomanized) ? null : info.NameRomanized;
                Author = info.Author;
                BPM = info.Bpm;
                Tags = info.SearchTags;
                LevelDesigner = info.LevelDesigner;
                BaseScene = Utils.SceneFromString(info.Scene);
                SceneEgg = info.SceneEgg;
                HiddenUnlockMethod = Utils.HiddenUnlockMethodFromString(info.HideBmsMode);
                HiddenUnlockType = Utils.HiddenUnlockTypeFromString(info.HideBmsDifficulty);
                HiddenUnlockMessage = string.IsNullOrWhiteSpace(info.HideBmsMessage) ? "Hidden difficulty!" : info.HideBmsMessage;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to refresh {Chart.Album.AlbumName} ChartInfo from the album");
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
                Log.Error($"Failed to save {Chart.Album.AlbumName} ChartInfo to the temp folder", ex);
            }
        }

        internal void SaveToAlbum()
        {
            try
            {
                var json = GetJson();
                SystemManager.WriteToAlbum(Chart.Album, "info.json", json.ToJsonString(Utils.JsonOptions));
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to save {Chart.Album.AlbumName} ChartInfo to the album", ex);
            }
        }

        internal ChartInfo(Chart chart)
        {
            Chart = chart;

            RefreshFromAlbum();
        }
    }
}
