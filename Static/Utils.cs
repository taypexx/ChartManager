using ChartManager.Data.Charts;
using Il2CppSirenix.Serialization.Utilities;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ChartManager.Static;

internal static class Utils
{
    public static Font NormalFont = Addressables.LoadAssetAsync<Font>("Normal").WaitForCompletion();

    public static readonly Dictionary<int, string> DifficultyNames = new()
    {
        [1] = "Easy",
        [2] = "Hard",
        [3] = "Master",
        [4] = "Hidden",
        [5] = "Touhou"
    };

    public static JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static ZipArchive GetNestedZip(this ZipArchive mdp, string entryName, ZipArchiveMode mode)
    {
        var mdm = mdp.GetEntry(entryName) ?? throw new ArgumentException($"Entry {entryName} not found.");
        using var mdmStream = mdm.Open();
        var openedMdm = new ZipArchive(mdmStream, mode, false);
        return openedMdm;
    }

    public static string GetNiceLine(string title, string value, Color? titleColor = null, Color? valueColor = null)
    {
        if (titleColor == null) titleColor = Color.white;
        if (valueColor == null) valueColor = Color.white;

        return $"<color={ColorToHex((Color)titleColor)}>[ {title} ]:</color> <color={ColorToHex((Color)valueColor)}>{value}</color>\n";
    }

    public static string StringFromScene(Scene scene) => $"scene_{(byte)scene:D2}";

    public static Scene SceneFromString(string scene)
    {
        return byte.TryParse(scene[^2..], out var sceneIndex)
        ? (
            Enum.IsDefined(typeof(Scene), sceneIndex)
            ? (Scene)sceneIndex
            : Scene.Unknown
        )
        : Scene.Unknown;
    }

    public static HiddenUnlockMethod HiddenUnlockMethodFromString(string hideBmsMode)
    {
        return Enum.TryParse<HiddenUnlockMethod>(hideBmsMode, out var method)
        ? method
        : HiddenUnlockMethod.CLICK;
    }

    public static HiddenUnlockType HiddenUnlockTypeFromString(string hideBmsDifficulty)
    {
        return Enum.TryParse<HiddenUnlockType>(hideBmsDifficulty, out var type)
        ? type
        : HiddenUnlockType.Default;
    }

    /// <summary>
    /// Rounds the <paramref name="value"/> to 2 decimal places.
    /// </summary>
    public static float RoundFloat(float value) => float.IsNaN(value) ? 0f : (float)Math.Round((decimal)(value * 100)) / 100;

    /// <summary>
    /// Checks if the <paramref name="str"/> is within the given bounds.
    /// </summary>
    public static bool IsValidString(string str, int minLength, int maxLength) => !str.IsNullOrWhitespace() && str.Length <= maxLength && str.Length >= minLength;

    /// <summary>
    /// Checks if the <paramref name="num"/> is within the given bounds.
    /// </summary>
    public static int? GetValidNumber(string num, int? min = null, int? max = null)
    {
        if (num.IsNullOrWhitespace()) return null;
        return !int.TryParse(num, out var num_) ? null : min != null && max != null && (num_ < min || num_ > max) ? null : num_;
    }

    public static string ColorToHex(Color color)
    {
        var r = Mathf.RoundToInt(color.r * 255f);
        var g = Mathf.RoundToInt(color.g * 255f);
        var b = Mathf.RoundToInt(color.b * 255f);

        return $"{r:X2}{g:X2}{b:X2}ff".ToLowerInvariant();
    }

    public static void StartProcess(string fileName)
    {
        try
        {
            _ = Process.Start(new ProcessStartInfo()
            {
                FileName = fileName,
                UseShellExecute = true
            });
        }
        catch { }
    }
}