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

        internal static DirectoryInfo MainDirectory;

        internal static string GetFromZip(ZipArchive zip, string fileName)
        {
            var entry = zip.GetEntry(fileName);
            using (var reader = new StreamReader(entry.Open()))
            {
                return reader.ReadToEnd();
            }
        }

        internal static void WriteToZip(ZipArchive zip, string fileName, string data)
        {
            var entry = zip.GetEntry(fileName);
            using (var writer = new StreamWriter(entry.Open()))
            {
                writer.Write(data);
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

        internal static void Init()
        {
            MainDirectory = Directory.CreateDirectory("UserData/" + Main.ModName);
        }

        internal static void Cleanup()
        {
            MainDirectory.Delete(recursive: true);
        }
    }
}
