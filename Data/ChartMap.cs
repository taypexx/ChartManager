using ChartManager.Managers;
using ChartManager.Static;
using Il2CppRewired.UI.ControlMapper;
using System.Text;

namespace ChartManager.Data
{
    internal class ChartMap
    {
        internal Chart Chart;
        internal string BmsName => $"map{Difficulty}.bms";
        internal byte Difficulty;
        internal string Level;
        internal string LevelDesigner;
        internal NoteSpeed InitialSpeed;
        internal Scene InitialScene;
        internal float InitialBPM;

        private void RefreshExtraFromData(string data)
        {
            var lines = data.Split('\n');

            try
            {
                var speedLine = lines.First((l) => l.StartsWith("#PLAYER"));
                InitialSpeed = Enum.TryParse(speedLine.Split(' ')[1], out NoteSpeed speed)
                    ? speed
                    : NoteSpeed.Speed2;
            }
            catch { InitialSpeed = NoteSpeed.Speed2; }

            try
            {
                var sceneLine = lines.First((l) => l.StartsWith("#GENRE"));
                InitialScene = Enum.TryParse(sceneLine.Split(' ')[1], out Scene scene)
                    ? scene
                    : Scene.SpaceStation;
            }
            catch { InitialScene = Scene.SpaceStation; }

            try
            {
                var bpmLine = lines.First((l) => l.StartsWith("#BPM"));
                InitialBPM = float.TryParse(bpmLine.Split(' ')[1], out var bpm)
                    ? bpm
                    : 120f;
            }
            catch { InitialBPM = 120f; }
        }

        internal void RefreshFromTemp()
        {
            try
            {
                var data = SystemManager.GetFromTemp(Chart, BmsName);
                RefreshExtraFromData(data);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to refresh {Chart.Album.AlbumName} {BmsName} from the temp folder", ex);
            }
        }

        internal void RefreshFromAlbum()
        {
            try
            {
                var info = Chart.Info;
                Level = info.Difficulties.TryGetValue(Difficulty, out var level) ? level : "?";

                string designer;
                switch (Difficulty)
                {
                    case 1:
                        designer = info.LevelDesigner1;
                        break;
                    case 2:
                        designer = info.LevelDesigner2;
                        break;
                    case 3:
                        designer = info.LevelDesigner3;
                        break;
                    case 4:
                        designer = info.LevelDesigner4;
                        break;
                    case 5:
                        designer = info.LevelDesigner5;
                        break;
                    default:
                        designer = Chart.Info.LevelDesigner;
                        break;
                }
                LevelDesigner = designer;

                using var bmsFile = album.OpenFileStreamIfPossible(BmsName);
                RefreshExtraFromStream(bmsFile);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to refresh {Chart.Album.AlbumName} {BmsName} from the album", ex);
            }
        }

        internal void SaveToTemp()
        {
            try
            {
                File.WriteAllText(TempPath, BmsData);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to save {Chart.Album.AlbumName} {BmsName} to the temp folder", ex);
            }
        }

        internal void SaveToAlbum()
        {
            try
            {
                SystemManager.WriteToAlbum(Chart.Album, BmsName, BmsData);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to save {Chart.Album.AlbumName} {BmsName} to the album", ex);
            }
        }

        internal ChartMap(Chart chart, int difficulty)
        {
            Chart = chart;
            Difficulty = (byte)difficulty;

            RefreshFromAlbum();
        }
    }
}
