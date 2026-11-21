using ChartManager.Static;
using UnityEngine;

namespace ChartManager.Data.Charts
{
    internal class ChartMap
    {
        internal Chart Chart;
        internal string BmsData;
        internal string BmsName => $"map{Difficulty}.bms";
        internal byte Difficulty;
        internal string Level => Chart.Info.Difficulties[Difficulty];
        internal string LevelDesigner => Chart.Info.LevelDesigners[Difficulty];
        internal NoteSpeed InitialSpeed;
        internal Scene InitialScene;
        internal float InitialBPM;

        public override string ToString()
        {
            return $"<color=#{Utils.ColorToHex(Color.white)}>| map{Difficulty}.bms |</color>\n"
                    + Utils.GetNiceLine("Difficulty", $"{Utils.DifficultyNames[Difficulty]} {Level}★", Palette.Yellow1)
                    + Utils.GetNiceLine("Level Designer", LevelDesigner, Palette.Yellow1)
                    + Utils.GetNiceLine("Initial Speed", ((byte)InitialSpeed).ToString(), Palette.Yellow1);
        }

        private void RefreshExtra()
        {
            var lines = BmsData.Split('\n');

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
                BmsData = Chart.GetFromTemp(BmsName);
                RefreshExtra();
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
                BmsData = Chart.GetFromAlbum(BmsName);
                RefreshExtra();
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
                Chart.WriteToTemp(BmsName, BmsData);
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
                Chart.WriteToAlbum(BmsName, BmsData);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to save {Chart.Album.AlbumName} {BmsName} to the album", ex);
            }
        }

        internal ChartMap(Chart chart, byte difficulty)
        {
            Chart = chart;
            Difficulty = difficulty;

            RefreshFromAlbum();
        }
    }
}
