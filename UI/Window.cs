using Il2CppAssets.Scripts.UI.Tips;
using LocalizeLib;
using PopupLib.UI.Components;
using PopupLib.UI.Windows;
using PopupLib.UI.Windows.Abstract;
using ChartManager.Data;
using ChartManager.Managers;

namespace ChartManager.UI
{
    internal abstract class Window
    {
        internal LocalString Title { get; set; }
        internal ForumWindow Base { get; private set; }

        internal ImageAsset Banner { get; private set; }
        internal Dictionary<ForumObject, object> ButtonsWindows { get; private set; }

        /// <summary>
        /// Adds a new button to the window.
        /// </summary>
        /// <param name="name">Text to be displayed on the button.</param>
        /// <param name="windowToOpen">(Optional) Window to open when the button is pressed.</param>
        /// <param name="content">(Optional) Text to be displayed in the frame.</param>
        /// <returns>A new button.</returns>
        protected ForumObject AddButton(string name, object? windowToOpen = null, string? content = null)
        {
            if (content == null) content = string.Empty;

            ForumObject button = new(new(name), new(content));
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

        internal void Show() => Base.Show();

        internal void Close() => Base.ForceClose();

        internal virtual void Refresh()
        {
            Base.ForceClose();
            Base.Show();
        }

        protected virtual void OnClick(PopupLib.UI.Windows.Interfaces.IListWindow _, int objectIndex)
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

        protected virtual void OnShow(BaseWindow _)
        {
            if (Title != null)
            {
                BulletinExtension.Title.text = Title.ToString();
            }
            BulletinExtension.CurrentWindow = this;
            BulletinExtension.Toggle(true);
        }

        protected virtual void OnClose(BaseWindow _)
        {
            BulletinExtension.CurrentWindow = null;
            BulletinExtension.Toggle(false);
        }

        /// <summary>
        /// Creates a new multiplayer window.
        /// </summary>
        /// <param name="title">Title of the window.</param>
        /// <param name="bannerName">(Optional) Path of the banner image asset.</param>
        internal Window(string title, string? bannerName = null)
        {
            Title = new(title);
            if (bannerName != null)
            {
                Banner = AssetManager.GetImageAsset("Banners." + bannerName);
            }

            Base = new();
            Base.AutoReset = true;

            Base.OnSelectionChanged += OnClick;
            Base.OnInternalShow += OnShow;
            Base.OnCompletion += OnClose;
        }
    }
}
