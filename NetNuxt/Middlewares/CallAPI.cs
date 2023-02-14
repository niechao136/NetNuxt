using System.Net.Http.Headers;

namespace NetNuxt.Middlewares;

public interface CallAPI
{
    private static readonly HttpClient client = httpClient();
    
    private static HttpClient httpClient()
    {
        HttpClientHandler handler = new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
        };
        HttpClient httpClient = new HttpClient(handler);
        
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        httpClient.Timeout = TimeSpan.FromSeconds(600);

        return httpClient;
    }

    protected static string CallWebAPI(string url, string body, string type = "application/json", string method = "POST")
    {
        var json = "";
        
        var httpContent = new StringContent(body);
        httpContent.Headers.ContentType = new MediaTypeHeaderValue(type);
        var task = method.ToUpper() == "GET" 
            ? client.GetAsync(url) 
            : client.PostAsync(url, httpContent);
        // var task = method.ToUpper() switch
        // {
        //     "POST" => client.PostAsync(url, httpContent),
        //     "GET" => client.GetAsync(url),
        //     _ => null
        // };
        task?.Wait();
        var response = task?.Result;
        if (response!.IsSuccessStatusCode)
        {
            var read = response.Content.ReadAsStringAsync();
            read.Wait();
            json = read.Result;
        }
        return json;
    }
}