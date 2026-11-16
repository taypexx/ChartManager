using ChartManager.Static;
using CustomAlbums.Albums;
using CustomAlbums.Data;
using System.IO.Compression;
using System.Windows.Forms;

namespace ChartManager.Managers
{
    internal enum FormatGroup
    {
        All, Cover, Demo, Music, Map, Info, Talk, Cinema, Video
    }

    internal static class SystemManager
    {
        private static Dictionary<FormatGroup, string> FormatGroupDescriptions = new()
        {
            [FormatGroup.All] = "Chart files (cover.png;cover.gif;demo.ogg;demo.mp3;music.ogg;music.mp3;map?.bms;info.json;map?.talk;cinema.json;*.mp4)|cover.png;cover.gif;demo.ogg;demo.mp3;music.ogg;music.mp3;map?.bms;info.json;map?.talk;cinema.json;*.mp4",
            [FormatGroup.Cover] = "Cover file (cover.png;cover.gif)|cover.png;cover.gif",
            [FormatGroup.Demo] = "Demo file (demo.ogg;demo.mp3)|demo.ogg;demo.mp3",
            [FormatGroup.Music] = "Music file (music.ogg;music.mp3)|music.ogg;music.mp3",
            [FormatGroup.Map] = "Map file (map?.bms)|map?.bms",
            [FormatGroup.Info] = "Info file (info.json)|info.json",
            [FormatGroup.Talk] = "Dialog file (map?.talk)|map?.talk",
            [FormatGroup.Cinema] = "Cinema config file (cinema.json)|cinema.json",
            [FormatGroup.Video] = "Cinema video file (*.mp4)|*.mp4"
        };

        private static DirectoryInfo MainDirectory;

        /// <returns>Path of the folder containing album files.</returns>
        internal static DirectoryInfo? CopyAlbumFiles(Album album)
        {
            try
            {
                var folder = MainDirectory.CreateSubdirectory(album.AlbumName);

                switch (album)
                {
                    case ArchiveAlbum:
                        ZipFile.ExtractToDirectory(album.Path, folder.FullName, overwriteFiles: true);
                        break;
                    case PackAlbum:
                        var mdp = ZipFile.OpenRead(album.Path);
                        var openedMdm = Utils.GetNestedZip(mdp, ((PackAlbum)album).PackAlbumName + ".mdm");
                        openedMdm.ExtractToDirectory(folder.FullName, overwriteFiles: true);
                        break;
                    case FolderAlbum:
                        foreach (string file in Directory.GetFiles(album.Path))
                        {
                            string destination = Path.Combine(folder.FullName, Path.GetFileName(file));

                            File.Copy(file, destination, overwrite: true);
                        }
                        break;
                    default:
                        throw new Exception("Unknown album type.");
                }

                return folder;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to copy album files of the {album.AlbumName}: {ex}");
            }
            return null;
        }

        internal static string? SelectFile(FormatGroup[] formats)
        {
            using (var dialog = new OpenFileDialog())
            {
                string filter = string.Empty;
                for (int i = 0; i < formats.Length; i++)
                {
                    filter += FormatGroupDescriptions[formats[i]];
                    if (i < formats.Length - 1) filter += "|";
                }

                dialog.Title = "Select a file";
                dialog.Filter = filter;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.FileName;
                }
            }
            return null;
        } 

        internal static void CreateMainFolder()
        {
            MainDirectory = Directory.CreateDirectory("UserData/" + Main.ModName);
        }

        internal static void Cleanup()
        {
            MainDirectory.Delete(recursive: true);
        }
    }
}
