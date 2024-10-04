using System.Net;

namespace AsyncTricks.Loader;

/// <summary>
///     Класс отправки запросов.
/// </summary>
public class RequestSenderBase : IDisposable
{
    private readonly HttpClient _client;

    protected RequestSenderBase()
    {
        var handler = new HttpClientHandler();
        var proxy = WebRequest.GetSystemWebProxy();
        proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
        handler.UseDefaultCredentials = true;
        handler.Proxy = proxy;
        _client = new HttpClient(handler);
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    /// <summary>
    ///     Оправить запрос.
    /// </summary>
    /// <param name="url">Урла.</param>
    /// <returns>Ответ.</returns>
    protected async Task<HttpResponseMessage> GetAsync(string url)
    {
        try
        {
            return await _client.GetAsync(new Uri(url));

            // вариант чтобы было более наглядна разница в скорости ислонения запросов.
            // await Task.Delay(1000);
            // return new HttpResponseMessage();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send request \n {ex.Message}{ex.Data}\n{ex.StackTrace}");
            throw;
        }
    }
}