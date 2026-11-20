namespace ChartManager.Data.Charts
{
    internal enum Scene : byte
    {
        Unknown, SpaceStation, Retrocity, Castle, RainyNight, Candyland, Oriental, GrooveCoaster, Touhou, DJMAX, Miku, AprilFools, Oriental2
    }

    internal enum NoteSpeed : byte
    {
        Speed1 = 1, Speed2 = 2, Speed3 = 3
    }

    internal enum HiddenUnlockMethod : byte
    {
        CLICK, PRESS, TOGGLE
    }

    internal enum HiddenUnlockType : sbyte
    {
        RemoveAll = -1,
        Default = 0,
        ReplaceEasy = 1,
        ReplaceHard = 2,
        ReplaceMaster = 3
    }
}
