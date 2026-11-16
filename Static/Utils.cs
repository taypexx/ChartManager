using Il2CppSirenix.Serialization.Utilities;
using System.Diagnostics;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ChartManager.Static
{
    internal static class Utils
    {
        public static Font NormalFont = Addressables.LoadAssetAsync<Font>("Normal").WaitForCompletion();

        public static ZipArchive GetNestedZip(this ZipArchive mdp, string entryName)
        {
            var mdm = mdp.GetEntry(entryName) ?? throw new ArgumentException($"Entry {entryName} not found.");
            var mdmStream = mdm.Open();
            var openedMdm = new ZipArchive(mdmStream, ZipArchiveMode.Read, false);
            return openedMdm;
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