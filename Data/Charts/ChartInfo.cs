using ChartManager.Static;
using CustomAlbums.Data;
using System.Text.Json;
using System.Text.Json.Nodes;
using UnityEngine;

namespace ChartManager.Data.Charts;

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
    internal Scene BaseScene;
    internal string[] Tags;
    internal Dictionary<byte, string> LevelDesigners;
    internal Dictionary<byte, string> Difficulties;
    internal HiddenUnlockMethod HiddenUnlockMethod;
    internal HiddenUnlockType HiddenUnlockType;
    internal string HiddenUnlockMessage;
    internal SceneEgg.SceneEggs SceneEgg;

    public override string ToString()
    {
        var str = $"<color={Utils.ColorToHex(Color.white)}>{Name}";
        if (NameRomanized != null)
        {
            str += $" ({NameRomanized})";
        }
        str += $" - {Author}</color>\n\n"

        + Utils.GetNiceLine("BPM", BPM, Color.white, Palette.Yellow1)
        + Utils.GetNiceLine("Scene", $"{Utils.StringFromScene(BaseScene)} ({BaseScene})", Color.white, Palette.Yellow1)
        + Utils.GetNiceLine("Tags", Tags.Length == 0 ? "-" : string.Join(", ", Tags), Color.white, Palette.Yellow1)
        + Utils.GetNiceLine("Level Designers", LevelDesigners.Values.All(d => d == LevelDesigners.Values.First()) ? LevelDesigners.Values.First() : string.Join(", ", LevelDesigners.Values), Color.white, Palette.Yellow1)
        + Utils.GetNiceLine("Difficulties", string.Join(", ", Difficulties.Values.Where(d => d != "0").Select(d => d += "★")), Color.white, Palette.Yellow1)
        + Utils.GetNiceLine("Hidden Unlock Method", HiddenUnlockMethod.ToString(), Color.white, Palette.Yellow1)
        + Utils.GetNiceLine("Hidden Unlock Type", $"{(sbyte)HiddenUnlockType} ({HiddenUnlockType})", Color.white, Palette.Yellow1)
        + Utils.GetNiceLine("Hidden Unlock Message", HiddenUnlockMessage, Color.white, Palette.Yellow1)
        + Utils.GetNiceLine("Scene Egg", SceneEgg.ToString(), Color.white, Palette.Yellow1);
        return str;
    }

    internal string GetJson()
    {
        var obj = new JsonObject()
        {
            ["name"] = Name,
            ["name_romanized"] = NameRomanized,
            ["author"] = Author,
            ["bpm"] = BPM,
            ["scene"] = Utils.StringFromScene(BaseScene),
            ["searchTags"] = new JsonArray(Tags.Select(t => JsonValue.Create(t)).ToArray()),
            ["levelDesigner"] = LevelDesigners[0],
            ["levelDesigner1"] = LevelDesigners[1],
            ["levelDesigner2"] = LevelDesigners[2],
            ["levelDesigner3"] = LevelDesigners[3],
            ["levelDesigner4"] = LevelDesigners[4],
            ["levelDesigner5"] = LevelDesigners[5],
            ["difficulty1"] = Difficulties[1],
            ["difficulty2"] = Difficulties[2],
            ["difficulty3"] = Difficulties[3],
            ["difficulty4"] = Difficulties[4],
            ["difficulty5"] = Difficulties[5],
            ["hideBmsMode"] = HiddenUnlockMethod.ToString(),
            ["hideBmsDifficulty"] = ((sbyte)HiddenUnlockType).ToString(),
            ["hideBmsMessage"] = HiddenUnlockMessage,
            ["sceneEgg"] = SceneEgg == CustomAlbums.Data.SceneEgg.SceneEggs.None ? null : SceneEgg.ToString()
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
        else
        {
            Name = Placeholder;
        }

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
        else
        {
            Author = Placeholder;
        }

        if (root.TryGetProperty("bpm", out var bpm))
        {
            var str = bpm.GetString();
            BPM = string.IsNullOrWhiteSpace(str) ? Placeholder : str;
        }
        else
        {
            BPM = Placeholder;
        }

        BaseScene = root.TryGetProperty("scene", out var scene) ? Utils.SceneFromString(scene.GetString()) : Scene.Unknown;

        Tags = root.TryGetProperty("searchTags", out var tags)
            ? JsonSerializer.Deserialize<string[]>(tags, Utils.JsonOptions) ?? new string[] { }
            : (new string[] { });

        if (root.TryGetProperty("levelDesigner", out var designer0))
        {
            var str = designer0.GetString();
            LevelDesigners[0] = string.IsNullOrWhiteSpace(str) ? Placeholder : str;
        }
        else
        {
            LevelDesigners[0] = Placeholder;
        }

        for (byte i = 1; i <= 5; i++)
        {
            if (root.TryGetProperty("levelDesigner" + i, out var designer))
            {
                var str = designer.GetString();
                LevelDesigners[i] = string.IsNullOrWhiteSpace(str) ? LevelDesigners[0] : str;
            }
            else
            {
                LevelDesigners[i] = LevelDesigners[0];
            }

            if (root.TryGetProperty("difficulty" + i, out var diff))
            {
                var str = diff.GetString();
                Difficulties[i] = string.IsNullOrWhiteSpace(str) ? "0" : str;
            }
            else
            {
                Difficulties[i] = "0";
            }
        }

        HiddenUnlockMethod = root.TryGetProperty("hideBmsMode", out var hideBmsMode)
            ? Utils.HiddenUnlockMethodFromString(hideBmsMode.GetString())
            : HiddenUnlockMethod.CLICK;

        HiddenUnlockType = root.TryGetProperty("hideBmsDifficulty", out var hideBmsDifficulty)
            ? Utils.HiddenUnlockTypeFromString(
                hideBmsDifficulty.ValueKind == JsonValueKind.String
                ? hideBmsDifficulty.GetString()
                : hideBmsDifficulty.GetSByte().ToString()
            )
            : HiddenUnlockType.Default;

        if (root.TryGetProperty("hideBmsMessage", out var hideBmsMessage))
        {
            var str = hideBmsMessage.GetString();
            HiddenUnlockMessage = string.IsNullOrWhiteSpace(str) ? HiddenPlaceholder : str;
        }
        else
        {
            HiddenUnlockMessage = HiddenPlaceholder;
        }

        if (root.TryGetProperty("sceneEgg", out var sceneEgg_))
        {
            if (Enum.TryParse(typeof(SceneEgg.SceneEggs), sceneEgg_.GetString(), out var sceneEgg))
            {
                SceneEgg = (SceneEgg.SceneEggs)sceneEgg;
            }
        }
        else
        {
            SceneEgg = CustomAlbums.Data.SceneEgg.SceneEggs.None;
        }
    }

    internal void RefreshFromTemp()
    {
        try
        {
            var json = Chart.GetFromTemp(InfoFile);
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
            var json = Chart.GetFromAlbum(InfoFile);
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
            Chart.WriteToTemp(InfoFile, json);
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
            Chart.WriteToAlbum(InfoFile, json);
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to save {Chart.Album.AlbumName} ChartInfo to the album", ex);
        }
    }

    internal ChartInfo(Chart chart)
    {
        Chart = chart;
        LevelDesigners = new();
        Difficulties = new();

        RefreshFromAlbum();
    }
}
