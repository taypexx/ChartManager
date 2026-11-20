using ChartManager.Managers;
using ChartManager.Static;
using CustomAlbums.Data;
using Il2Cpprail;
using Il2CppSirenix.Serialization.Utilities;
using System.Text.Json;
using System.Text.Json.Nodes;
using static MelonLoader.Modules.MelonModule;

namespace ChartManager.Data
{
    internal class ChartInfo
    {
        private const string InfoFile = "info.json";
        private const string Placeholder = "Unknown";
        private const string HiddenPlaceholder = "Hidden difficulty!";

        internal Chart Chart;
        internal string Name;
        internal string? NameRomanized;
        internal string Author;
        internal string BPM;
        internal string[] Tags;
        internal string LevelDesigner;
        internal string[] LevelDesigners;
        internal string LevelDesigner1;
        internal string LevelDesigner2;
        internal string LevelDesigner3;
        internal string LevelDesigner4;
        internal string LevelDesigner5;
        internal 
        internal Scene BaseScene;
        internal SceneEgg.SceneEggs SceneEgg;
        internal HiddenUnlockMethod HiddenUnlockMethod;
        internal HiddenUnlockType HiddenUnlockType;
        internal string HiddenUnlockMessage;

        internal string GetJson()
        {
            var obj = new JsonObject()
            {
                ["name"] = Name,
                ["name_romanized"] = NameRomanized,
                ["author"] = Author,
                ["bpm"] = BPM,
                ["scene"] = $"scene_{((byte)BaseScene).ToString("D2")}",
                ["searchTags"] = new JsonArray(Tags.Select(t => JsonValue.Create(t)).ToArray()),
                ["levelDesigner"] = LevelDesigner,
                ["levelDesigner1"] = LevelDesigner1,
                ["levelDesigner2"] = LevelDesigner2,
                ["levelDesigner3"] = LevelDesigner3,
                ["levelDesigner4"] = LevelDesigner4,
                ["levelDesigner5"] = LevelDesigner5,
                ["difficulty1"] = map1 != null ? map1.Level : "0",
                ["difficulty2"] = map2 != null ? map2.Level : "0",
                ["difficulty3"] = map3 != null ? map3.Level : "0",
                ["difficulty4"] = map4 != null ? map4.Level : "0",
                ["difficulty5"] = map5 != null ? map5.Level : "0",
                ["hideBmsMode"] = HiddenUnlockMethod.ToString(),
                ["hideBmsDifficulty"] = ((sbyte)HiddenUnlockType).ToString(),
                ["hideBmsMessage"] = HiddenUnlockMessage
            };
            return obj.ToJsonString(Utils.JsonOptions);
        }

        private void RefreshFromJson(string rawJson)
        {
            var json = JsonSerializer.Deserialize<JsonDocument>(rawJson, Utils.JsonOptions);
            if (json == null)
            {
                Log.Error($"Failed to parse {InfoFile}.");
                return;
            }

            var root = json.RootElement;

            if (root.TryGetProperty("name", out var name))
            {
                var str = name.ToString();
                Name = string.IsNullOrWhiteSpace(str) ? Placeholder : str;
            }
            else Name = Placeholder;

            if (root.TryGetProperty("name_romanized", out var name_rom))
            {
                var str = name_rom.GetString();
                NameRomanized = string.IsNullOrWhiteSpace(str) ? null : str;
            }

            if (root.TryGetProperty("author", out var author))
            {
                var str = author.GetString();
                Author = string.IsNullOrWhiteSpace(str) ? Placeholder : str;
            }
            else Author = Placeholder;

            if (root.TryGetProperty("bpm", out var bpm))
            {
                var str = bpm.GetString();
                BPM = string.IsNullOrWhiteSpace(str) ? Placeholder : str;
            }
            else BPM = Placeholder;

            if (root.TryGetProperty("searchTags", out var tags))
            {
                Tags = JsonSerializer.Deserialize<string[]>(tags, Utils.JsonOptions) ?? new string[] {};
            }
            else Tags = new string[] { };

            if (root.TryGetProperty("levelDesigner", out var designer))
            {
                var str = designer.GetString();
                LevelDesigner = string.IsNullOrWhiteSpace(str) ? Placeholder : str;
            }
            else LevelDesigner = Placeholder;

            if (root.TryGetProperty("levelDesigner1", out var designer1))
            {
                var str = designer1.GetString();
                LevelDesigner1 = string.IsNullOrWhiteSpace(str) ? null : str;
            }

            if (root.TryGetProperty("levelDesigner2", out var designer2))
            {
                var str = designer2.GetString();
                LevelDesigner2 = string.IsNullOrWhiteSpace(str) ? null : str;
            }

            if (root.TryGetProperty("levelDesigner3", out var designer3))
            {
                var str = designer3.GetString();
                LevelDesigner3 = string.IsNullOrWhiteSpace(str) ? null : str;
            }

            if (root.TryGetProperty("levelDesigner4", out var designer4))
            {
                var str = designer4.GetString();
                LevelDesigner4 = string.IsNullOrWhiteSpace(str) ? null : str;
            }

            if (root.TryGetProperty("levelDesigner5", out var designer5))
            {
                var str = designer5.GetString();
                LevelDesigner5 = string.IsNullOrWhiteSpace(str) ? null : str;
            }

            if (root.TryGetProperty("scene", out var scene))
            {
                BaseScene = Utils.SceneFromString(scene.GetString());
            }
            else BaseScene = Scene.Unknown;

            if (root.TryGetProperty("sceneEgg", out var sceneEgg_))
            {
                Enum.TryParse(typeof(SceneEgg.SceneEggs), sceneEgg_.GetString(), out var sceneEgg);
                SceneEgg = (SceneEgg.SceneEggs)sceneEgg;
            }
            else SceneEgg = CustomAlbums.Data.SceneEgg.SceneEggs.None;

            if (root.TryGetProperty("hideBmsMode", out var hideBmsMode))
            {
                HiddenUnlockMethod = Utils.HiddenUnlockMethodFromString(hideBmsMode.GetString());
            }
            else HiddenUnlockMethod = HiddenUnlockMethod.CLICK;

            if (root.TryGetProperty("hideBmsDifficulty", out var hideBmsDifficulty))
            {
                HiddenUnlockType = Utils.HiddenUnlockTypeFromString(hideBmsDifficulty.GetString());
            }
            else HiddenUnlockType = HiddenUnlockType.Default;

            if (root.TryGetProperty("hideBmsMessage", out var hideBmsMessage))
            {
                var str = hideBmsMessage.GetString();
                HiddenUnlockMessage = string.IsNullOrWhiteSpace(str) ? HiddenPlaceholder : str;
            }
            else HiddenUnlockMessage = HiddenPlaceholder;
        }

        internal void RefreshFromTemp()
        {
            try
            {
                var json = SystemManager.GetFromTemp(Chart, InfoFile);
                RefreshFromJson(json);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to refresh {Chart.Album.AlbumName} ChartInfo from the temp folder", ex);
            }
        }

        internal void RefreshFromAlbum()
        {
            try
            {
                var json = SystemManager.GetFromAlbum(Chart.Album, InfoFile);
                RefreshFromJson(json);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to refresh {Chart.Album.AlbumName} ChartInfo from the album", ex);
            }
        }

        internal void SaveToTemp()
        {
            try
            {
                var json = GetJson();
                SystemManager.WriteToTemp(Chart, InfoFile, json);
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
                SystemManager.WriteToAlbum(Chart.Album, InfoFile, json);
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
