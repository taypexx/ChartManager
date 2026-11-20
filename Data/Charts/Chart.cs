using ChartManager.Managers;
using ChartManager.Static;
using CustomAlbums.Albums;
using CustomAlbums.Data;
using Il2CppAssets.Scripts.Database;
using System.IO.Compression;
using System.Net.Http.Json;
using System.Text.Json;

namespace ChartManager.Data.Charts
{
    internal class Chart
    {
        private const string HitsoundsPath = $"{Main.ModName}.Assets.Template.WAVs.";

        internal Album Album;
        internal MusicInfo MusicInfo => GlobalDataBase.dbMusicTag.GetMusicInfoFromAll(Album.Uid);

        internal ChartAssets Assets;
        internal ChartInfo Info;
        internal Dictionary<int, ChartMap> Maps;
        internal ChartCinema? Cinema;
        internal bool HasCinema => Cinema != null;

        private bool? OnWebsite;
        private string? WebsiteId;
        private bool? IsRanked;

        private DirectoryInfo TempFolder;

        internal DirectoryInfo GetTempFolder()
        {
            if (TempFolder == null)
            {
                CopyAlbumFiles();
            }
            return TempFolder;
        }

        private void CopyAlbumFiles()
        {
            try
            {
                var folder = SystemManager.MainDirectory.CreateSubdirectory(Album.AlbumName);

                switch (Album)
                {
                    case ArchiveAlbum:
                        ZipFile.ExtractToDirectory(Album.Path, folder.FullName, overwriteFiles: true);
                        break;
                    case PackAlbum:
                        using (var mdp = ZipFile.OpenRead(Album.Path))
                        {
                            using (var openedMdm = Utils.GetNestedZip(mdp, ((PackAlbum)Album).PackAlbumName + ".mdm", ZipArchiveMode.Read))
                            {
                                openedMdm.ExtractToDirectory(folder.FullName, overwriteFiles: true);
                            }
                        }
                        break;
                    case FolderAlbum:
                        foreach (string file in Directory.GetFiles(Album.Path))
                        {
                            string destination = Path.Combine(folder.FullName, Path.GetFileName(file));
                            File.Copy(file, destination, overwrite: true);
                        }
                        break;
                    default:
                        throw new Exception("Unknown album type.");
                }

                TempFolder = folder;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to copy album files of the {Album.AlbumName}: {ex}");
            }
        }

        private void CopyHitsounds()
        {
            foreach (var resourceName in Main.CurrentAssembly.GetManifestResourceNames().Where(n => n.StartsWith(HitsoundsPath)))
            {
                using var stream = Main.CurrentAssembly.GetManifestResourceStream(resourceName);
                if (stream == null) continue;

                var path = Path.Combine(TempFolder.FullName, resourceName.Substring(HitsoundsPath.Length));
                using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                stream.CopyTo(fileStream);
            }
        }

        internal string? GetFromTemp(string fileName)
        {
            try
            {
                var path = Path.Combine(GetTempFolder().FullName, fileName);
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException("File doesn't exist.");
                }

                return File.ReadAllText(path);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get {fileName} from the {Album.AlbumName} temp folder: {ex}");
            }
            return null;
        }

        internal void WriteToTemp(string fileName, string data)
        {
            try
            {
                var path = Path.Combine(GetTempFolder().FullName, fileName);
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException("File doesn't exist.");
                }

                File.WriteAllText(path, data);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to write {fileName} to the {Album.AlbumName} temp folder: {ex}");
            }
        }

        internal string? GetFromAlbum(string fileName)
        {
            try
            {
                if (!Album.HasFile(fileName))
                {
                    throw new FileNotFoundException("File doesn't exist.");
                }

                switch (Album)
                {
                    case ArchiveAlbum:
                        using (var mdm = ZipFile.Open(Album.Path, ZipArchiveMode.Read))
                        {
                            return SystemManager.GetFromZip(mdm, fileName);
                        }
                    case PackAlbum:
                        using (var mdp = ZipFile.Open(Album.Path, ZipArchiveMode.Read))
                        {
                            using (var mdm = Utils.GetNestedZip(mdp, ((PackAlbum)Album).PackAlbumName + ".mdm", ZipArchiveMode.Read))
                            {
                                return SystemManager.GetFromZip(mdm, fileName);
                            }
                        }
                    case FolderAlbum:
                        var path = Path.Combine(Album.Path, fileName);
                        return File.ReadAllText(path);
                    default:
                        throw new Exception("Unknown album type.");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to write {fileName} to the {Album.AlbumName}: {ex}");
            }
            return null;
        }

        internal void WriteToAlbum(string fileName, string data)
        {
            try
            {
                if (!Album.HasFile(fileName))
                {
                    throw new FileNotFoundException("File doesn't exist.");
                }

                switch (Album)
                {
                    case ArchiveAlbum:
                        using (var mdm = ZipFile.Open(Album.Path, ZipArchiveMode.Update))
                        {
                            SystemManager.WriteToZip(mdm, fileName, data);
                        }
                        break;
                    case PackAlbum:
                        using (var mdp = ZipFile.Open(Album.Path, ZipArchiveMode.Update))
                        {
                            using (var mdm = Utils.GetNestedZip(mdp, ((PackAlbum)Album).PackAlbumName + ".mdm", ZipArchiveMode.Update))
                            {
                                SystemManager.WriteToZip(mdm, fileName, data);
                            }
                        }
                        break;
                    case FolderAlbum:
                        var path = Path.Combine(Album.Path, fileName);
                        File.WriteAllText(path, data);
                        break;
                    default:
                        throw new Exception("Unknown album type.");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to write {fileName} to the {Album.AlbumName}: {ex}");
            }
        }

        internal bool IsOnWebsite()
        {
            return OnWebsite == null ? false : (bool)OnWebsite;
        }

        internal async Task<bool> IsOnWebsiteAsync(bool forceUpdate = true)
        {
            if (forceUpdate || OnWebsite == null)
            {
                await UpdateWebsiteInfo();
            }
            return (bool)OnWebsite;
        }

        internal async Task UpdateWebsiteInfo()
        {
            using var response = await Client.GetAsync("https://api.mdmc.moe/v3/sheets/" + Album.Sheets[2].Md5, false);

            // We check for the 404 specifically, because the server might be down or anything
            if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                OnWebsite = response.IsSuccessStatusCode;
                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadFromJsonAsync<Dictionary<string, JsonElement>>();
                    var chartData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(body["chart"]);
                    WebsiteId = chartData["id"].GetString();
                    IsRanked = chartData["ranked"].GetBoolean();
                }
            }
            else OnWebsite = false;
        }

        internal Chart(Album album)
        {
            Album = album;
            Assets = new(this);
            Info = new(this);

            Maps = new();
            foreach (var (diff, sheet) in album.Sheets)
            {
                Maps.Add(diff, new(this, (byte)diff));
            }

            if (album.HasFile("cinema.json"))
            {
                Cinema = new(this);
            }
        }
    }
}
