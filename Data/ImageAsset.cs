using UnityEngine;

namespace ChartManager.Data
{
    internal sealed class ImageAsset
    {
        internal string Name { get; private set; }
        internal Texture2D Texture { get; private set; }
        internal Sprite Sprite { get; private set; }

        internal ImageAsset(byte[] bytes, string name = "Image")
        {
            Name = name;

            // Creating the texture
            Texture = new Texture2D(2, 2, TextureFormat.ARGB32, false)
            {
                wrapMode = TextureWrapMode.MirrorOnce
            };
            ImageConversion.LoadImage(Texture, bytes);

            // Creating the sprite
            Sprite = Sprite.Create(Texture, new Rect(0, 0, Texture.width, Texture.height), new Vector2(0.5f, 0.5f));

            // Prevent garbage collection
            UnityEngine.Object.DontDestroyOnLoad(Texture);
            UnityEngine.Object.DontDestroyOnLoad(Sprite);
        }
    }
}
