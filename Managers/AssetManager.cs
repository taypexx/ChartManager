using ChartManager.Data;
using UnityEngine;
using UnityEngine.UI;

namespace ChartManager.Managers;

internal static class AssetManager
{
    private static Dictionary<string, ImageAsset> ImageAssets;
    private static GameObject AssetHolder;

    /// <summary>
    /// Caches the image asset and holds the asset.
    /// </summary>
    /// <param name="imageAsset"><see cref="ImageAsset"/> to cache.</param>
    private static void CacheImageAsset(ImageAsset imageAsset)
    {
        var name = imageAsset.Name;

        // Return if cached already
        if (ImageAssets.ContainsKey(name)) return;

        ImageAssets.Add(name, imageAsset);

        GameObject go = new(name);
        go.transform.SetParent(AssetHolder.transform);
        go.AddComponent<Image>().sprite = imageAsset.Sprite;
    }

    /// <summary>
    /// Gets the <see cref="ImageAsset"/> reference or creates a new one and caches it.
    /// </summary>
    /// <param name="path">Path of the image.</param>
    /// <returns><see cref="ImageAsset"/> reference.</returns>
    internal static ImageAsset? GetImageAsset(string path)
    {
        if (ImageAssets.TryGetValue(path, out var asset)) return asset;

        using var stream = Main.CurrentAssembly.GetManifestResourceStream($"{Main.ModName}.Assets.{path}");
        if (stream == null) return null;

        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);

        var bytes = memoryStream.ToArray();

        ImageAsset newAsset = new(bytes, path);
        CacheImageAsset(newAsset);

        return newAsset;
    }

    internal static void Init()
    {
        AssetHolder = new(Main.ModName + "Assets");
        UnityEngine.Object.DontDestroyOnLoad(AssetHolder);

        ImageAssets = new();
    }
}