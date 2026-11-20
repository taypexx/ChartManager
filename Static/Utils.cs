using ChartManager.Data;
using Il2CppSirenix.Serialization.Utilities;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ChartManager.Static
{
    internal static class Utils
    {
        public static Font NormalFont = Addressables.LoadAssetAsync<Font>("Normal").WaitForCompletion();

        public static JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static ZipArchive GetNestedZip(this ZipArchive mdp, string entryName, ZipArchiveMode mode)
        {
            var mdm = mdp.GetEntry(entryName) ?? throw new ArgumentException($"Entry {entryName} not found.");
            using var mdmStream = mdm.Open();
            var openedMdm = new ZipArchive(mdmStream, mode, false);
            return openedMdm;
        }

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
        public static float RoundFloat(float value)
        {
            if (float.IsNaN(value)) return 0f;
            return (float)Math.Round((decimal)(value * 100)) / 100;
        }

        /// <summary>
        /// Checks if the <paramref name="str"/> is within the given bounds.
        /// </summary>
        public static bool IsValidString(string str, int minLength, int maxLength)
        {
            if (str.IsNullOrWhitespace()) return false;
            return str.Length <= maxLength && str.Length >= minLength;
        }

        /// <summary>
        /// Checks if the <paramref name="num"/> is within the given bounds.
        /// </summary>
        public static int? GetValidNumber(string num, int? min = null, int? max = null)
        {
            if (num.IsNullOrWhitespace()) return null;
            if (!Int32.TryParse(num, out int num_)) return null;
            if (min != null && max != null && (num_ < min || num_ > max)) return null;
            return num_;
        }

        /// <summary>
        /// Opens a <paramref name="url"/> in the default browser.
        /// </summary>
        public static void OpenBrowserLink(string url)
        {
            try { Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }); }
            catch { }
        }
    }
}