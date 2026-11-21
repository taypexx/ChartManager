using ChartManager.Data;
using ChartManager.Managers;
using ChartManager.UI.Extensions;
using Il2CppAssets.Scripts.UI.Tips;
using LocalizeLib;
using PopupLib.UI.Components;
using PopupLib.UI.Windows;
using PopupLib.UI.Windows.Abstract;

namespace ChartManager.UI.Windows
{
    internal abstract class Window
    {
        internal LocalString Title { get; set; }
        internal LocalString Content { get; private set; }
        internal ForumWindow Base { get; private set; }
        internal bool HasBackButton { get; private set; }
        internal Dictionary<ForumObject, object> ButtonsWindows { get; private set; }
        internal ImageAsset Banner { get; private set; }

        protected void UpdateContent(string text)
        {
            Content = new(text);
            foreach (var button in Base.ForumObjects)
            {
                button.Contents = Content;
            }
        }

        protected void UpdateBanner(string bannerName) => UpdateBanner(AssetManager.GetImageAsset("Banners." + bannerName));
        protected void UpdateBanner(ImageAsset banner)
        {
            Banner = banner;
            foreach (var button in Base.ForumObjects)
            {
                button.Texture = Banner.Texture;
            }
        }

        internal abstract void CreateButtons();

        protected void RemoveButtons()
        {
            while (Base.ForumObjects.Count > 0)
            {
                RemoveButton(Base.ForumObjects[0]);
            }
        }

        /// <summary>
        /// Adds a new button to the window.
        /// </summary>
        /// <param name="name">Text to be displayed on the button.</param>
        /// <param name="windowToOpen">(Optional) Window to open when the button is pressed.</param>
        /// <returns>A new button.</returns>
        protected ForumObject AddButton(string name, object? windowToOpen = null)
        {
            ForumObject button = new(new(name), Content);
            if (Banner != null)
            {
                button.Texture = Banner.Texture;
            }

            Base.ForumObjects.Add(button);
            ButtonsWindows.Add(button, windowToOpen);

            return button;
        }

        /// <summary>
        /// Removes the <see cref="ForumObject"/> <paramref name="button"/>.
        /// </summary>
        /// <returns><see langword="true"/> if it was removed, otherwise <see langword="false"/>.</returns>
        protected bool RemoveButton(ForumObject button)
        {
            if (button is null) return false;
            return Base.ForumObjects.Remove(button) && ButtonsWindows.Remove(button);
        }

        internal virtual void Show() => Base.Show();

        internal virtual void Close() => Base.ForceClose();

        internal virtual void Refresh()
        {
            /* Might be:
            Close();
            Show();
            */
            Base.ForceClose();
            Base.Show();
        }

        internal virtual void OnClick(PopupLib.UI.Windows.Interfaces.IListWindow _, int objectIndex)
        {
            if (!Base.Activated) return;
            ForumObject button = Base.ForumObjects[objectIndex];

            Base.ForceClose();

            var windowToOpen = ButtonsWindows[button];
            switch (windowToOpen)
            {
                case Window:
                    ((Window)windowToOpen).Show();
                    break;
                case BaseWindow:
                    ((BaseWindow)windowToOpen).Show();
                    break;
                case AbstractMessageBox:
                    ((AbstractMessageBox)windowToOpen).Show();
                    break;
            }
        }

        internal virtual void OnShow(BaseWindow _)
        {
            BulletinExtension.SetWindow(this, Title.ToString(), HasBackButton);
        }

        internal virtual void OnClose(BaseWindow _)
        {
            BulletinExtension.CloseWindow();
        }

        /// <summary>
        /// Creates a new multiplayer window.
        /// </summary>
        /// <param name="title">Title of the window.</param>
        /// <param name="bannerName">(Optional) Path of the banner image asset.</param>
        internal Window(string title, string? bannerName = null, bool hasBackButton = true)
        {
            Title = new(title);
            Content = new();
            HasBackButton = hasBackButton;
            ButtonsWindows = new();

            if (bannerName != null) UpdateBanner(bannerName);

            Base = new();
            Base.AutoReset = true;

            Base.OnSelectionChanged += OnClick;
            Base.OnInternalShow += OnShow;
            Base.OnCompletion += OnClose;
        }
    }
}
