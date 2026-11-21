using System.Collections.Concurrent;

namespace ChartManager.Static;

internal static class Dispatcher
{
    internal delegate void DispatcherCallbackDelegate();
    private static readonly ConcurrentQueue<DispatcherCallbackDelegate> ThreadQueue = new();

    internal static void Do(DispatcherCallbackDelegate del) => ThreadQueue.Enqueue(del);

    internal static void Update()
    {
        while (ThreadQueue.TryDequeue(out var f))
        {
            try
            {
                f?.Invoke();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}