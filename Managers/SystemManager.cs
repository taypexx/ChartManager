using ChartManager.Data;
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
            [FormatGroup.All] = "Chart files (cover.png;demo.ogg;demo.mp3;music.ogg;music.mp3;map?.bms;info.json;map?.talk;cinema.json;*.mp4)|cover.png;demo.ogg;demo.mp3;music.ogg;music.mp3;map?.bms;info.json;map?.talk;cinema.json;*.mp4",
            [FormatGroup.Cover] = "Cover file (cover.png)|cover.png",
            [FormatGroup.Demo] = "Demo file (demo.ogg;demo.mp3)|demo.ogg;demo.mp3",
            [FormatGroup.Music] = "Music file (music.ogg;music.mp3)|music.ogg;music.mp3",
            [FormatGroup.Map] = "Map file (map?.bms)|map?.bms",
            [FormatGroup.Info] = "Info file (info.json)|info.json",
            [FormatGroup.Talk] = "Dialog file (map?.talk)|map?.talk",
            [FormatGroup.Cinema] = "Cinema config file (cinema.json)|cinema.json",
            [FormatGroup.Video] = "Cinema video file (*.mp4)|*.mp4"
        };

        private static DirectoryInfo MainDirectory;
        private static readonly string HitsoundsPath = $"{Main.ModName}.Assets.Template.WAVs.";

        internal static void CopyHitsounds(DirectoryInfo folder)
        {
            foreach (var resourceName in Main.CurrentAssembly.GetManifestResourceNames().Where(n => n.StartsWith(HitsoundsPath)))
            {
                using var stream = Main.CurrentAssembly.GetManifestResourceStream(resourceName);
                if (stream == null) continue;

                var path = Path.Combine(folder.FullName, resourceName.Substring(HitsoundsPath.Length));
                using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                stream.CopyTo(fileStream);
            }
        }

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
                        using (var mdp = ZipFile.OpenRead(album.Path))
                        {
                            using (var openedMdm = Utils.GetNestedZip(mdp, ((PackAlbum)album).PackAlbumName + ".mdm", ZipArchiveMode.Read))
                            {
                                openedMdm.ExtractToDirectory(folder.FullName, overwriteFiles: true);
                            }
                        }
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

        private static string GetFromZip(ZipArchive zip, string fileName)
        {
            var entry = zip.GetEntry(fileName);
            using (var reader = new StreamReader(entry.Open()))
            {
                return reader.ReadToEnd();
            }
        }

        internal static string? GetFromAlbum(Album album, string fileName)
        {
            try
            {
                if (!album.HasFile(fileName))
                {
                    throw new FileNotFoundException("File doesn't exist.");
                }

                switch (album)
                {
                    case ArchiveAlbum:
                        using (var mdm = ZipFile.Open(album.Path, ZipArchiveMode.Read))
                        {
                            return GetFromZip(mdm, fileName);
                        }
                    case PackAlbum:
                        using (var mdp = ZipFile.Open(album.Path, ZipArchiveMode.Read))
                        {
                            using (var mdm = Utils.GetNestedZip(mdp, ((PackAlbum)album).PackAlbumName + ".mdm", ZipArchiveMode.Read))
                            {
                                return GetFromZip(mdm, fileName);
                            }
                        }
                    case FolderAlbum:
                        var path = Path.Combine(album.Path, fileName);
                        return File.ReadAllText(path);
                    default:
                        throw new Exception("Unknown album type.");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to write {fileName} to the {album.AlbumName}: {ex}");
            }
            return null;
        }

        private static void WriteToZip(ZipArchive zip, string fileName, string data)
        {
            var entry = zip.GetEntry(fileName);
            using (var writer = new StreamWriter(entry.Open()))
            {
                writer.Write(data);
            }
        }

        internal static void WriteToAlbum(Album album, string fileName, string data)
        {
            try
            {
                if (!album.HasFile(fileName))
                {
                    throw new FileNotFoundException("File doesn't exist.");
                }

                switch (album)
                {
                    case ArchiveAlbum:
                        using (var mdm = ZipFile.Open(album.Path, ZipArchiveMode.Update))
                        {
                            WriteToZip(mdm, fileName, data);
                        }
                        break;
                    case PackAlbum:
                        using (var mdp = ZipFile.Open(album.Path, ZipArchiveMode.Update))
                        {
                            using (var mdm = Utils.GetNestedZip(mdp, ((PackAlbum)album).PackAlbumName + ".mdm", ZipArchiveMode.Update))
                            {
                                WriteToZip(mdm, fileName, data);
                            }
                        }
                        break;
                    case FolderAlbum:
                        var path = Path.Combine(album.Path, fileName);
                        File.WriteAllText(path, data);
                        break;
                    default:
                        throw new Exception("Unknown album type.");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to write {fileName} to the {album.AlbumName}: {ex}");
            }
        }

        internal static string? GetFromTemp(Chart chart, string fileName)
        {
            try
            {
                var path = Path.Combine(chart.GetTempFolder().FullName, fileName);
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException("File doesn't exist.");
                }

                return File.ReadAllText(path);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get {fileName} from the {chart.Album.AlbumName} temp folder: {ex}");
            }
            return null;
        }

        internal static void WriteToTemp(Chart chart, string fileName, string data)
        {
            try
            {
                var path = Path.Combine(chart.GetTempFolder().FullName, fileName);
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException("File doesn't exist.");
                }

                File.WriteAllText(path, data);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to write {fileName} to the {chart.Album.AlbumName} temp folder: {ex}");
            }
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
