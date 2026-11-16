namespace ChartManager.Static
{
    internal static class Client
    {
        private static HttpClient Http = new();

        internal static async Task<HttpResponseMessage?> GetAsync(string address)
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
                Log.Error($"[{(int)response.StatusCode}] GET request failed: {await response.Content.ReadAsStringAsync()}");
                return null;
            }
        }
    }
}
