public class MockHttpMessageHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent("{ \"results\": [{ \"id\": 1, \"name\": \"Rick Sanchez\", \"status\": \"Alive\" }] }")
        };

        return Task.FromResult(response);
    }
}