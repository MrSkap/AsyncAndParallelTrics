namespace AsyncTricks.Loader;

/// <summary>
///     Отправляет запросы и пишет размер ответа.
/// </summary>
public class SizeOfResponseExtractor : RequestSenderBase
{
    /// <summary>
    ///     Отправить запросы по очереди.
    /// </summary>
    /// <param name="urls">Урлы.</param>
    public async Task LoadOneAfterOne(IEnumerable<string> urls)
    {
        foreach (var url in urls)
        {
            var response = await GetAsync(url);
            await response.Content.LoadIntoBufferAsync();
            Console.WriteLine(
                $"Get response from {url}. Size of response: {response.Content.Headers.ContentLength} bytes");
        }
    }

    /// <summary>
    ///     Оправить запросы все разом и вернутся, когда все запросы выполнятся.
    /// </summary>
    /// <param name="urls"></param>
    public async Task LoadWithWhenAll(IEnumerable<string> urls)
    {
        var tasks = urls.ToDictionary(SendRequestAndGetContentSize, x => x);
        await Task.WhenAll(tasks.Keys);
        foreach (var task in tasks)
        {
            var response = await task.Key;
            Console.WriteLine($"Get response from {task.Value}. Size of response: {response} bytes");
        }
    }

    /// <summary>
    ///     Отправить запросы все разом и реагировать при получении ответов.
    /// </summary>
    /// <param name="urls"></param>
    public async Task LoadWithWhenAny(IEnumerable<string> urls)
    {
        var tasks = urls.ToDictionary(SendRequestAndGetContentSize, x => x);
        while (tasks.Any())
        {
            var completedTask = await Task.WhenAny(tasks.Keys);
            var size = await completedTask;
            Console.WriteLine($"Get response from {tasks[completedTask]}. Size of response: {size} bytes");
            tasks.Remove(completedTask);
        }
    }

    private async Task<long> SendRequestAndGetContentSize(string url)
    {
        var response = await GetAsync(url);
        await response.Content.LoadIntoBufferAsync();
        return response.Content.Headers.ContentLength ?? 0;
    }
}