using MelonLoader;
using MelonLoader.Logging;

namespace ChartManager.Static;

internal static class Log
{
    private static readonly MelonLogger.Instance Logger;

    internal static void Info(string msg) => Logger.Msg(msg);

    internal static void Success(string msg) => Logger.Msg(ConsoleColor.Green, msg);

    internal static void Warning(string msg) => Logger.Warning(msg);

    internal static void Error(string msg) => Logger.Error(msg);

    internal static void Error(string msg, Exception ex) => Logger.Error(msg, ex);

    static Log()
    {
        Logger = new(Main.ModName, ColorARGB.Magenta);
    }
}
