namespace ChartManager.Static;

internal static class Client
{
    private static readonly HttpClient Http = new();

    internal static async Task<HttpResponseMessage?> GetAsync(string address, bool logErrors = true)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, address);
        var response = await Http.SendAsync(request);
        request.Dispose();

        if (response.IsSuccessStatusCode)
        {
            return response;
        }
        else
        {
            if (logErrors) Log.Error($"[{(int)response.StatusCode}] GET request failed: {await response.Content.ReadAsStringAsync()}");
            return null;
        }
    }
}
