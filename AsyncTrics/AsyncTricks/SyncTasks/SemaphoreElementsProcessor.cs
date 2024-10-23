namespace AsyncTricks.SyncTasks;

/// <summary>
///     Синхронный обработчик элеменнов через SemaphoreSlim.
/// </summary>
public class SemaphoreElementsProcessor : ElementsProcessor, IDisposable
{
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    /// <inheritdoc />
    public void Dispose()
    {
        _semaphoreSlim.Dispose();
    }

    /// <inheritdoc />
    public override async Task ProcessElementsAsync()
    {
        Console.WriteLine("Process elements in semaphore");

        await _semaphoreSlim.WaitAsync();
        try
        {
            while (ProcessElements.TryDequeue(out var element))
            {
                ProcessedElements.Add(element);
                Console.WriteLine($"Name - {element.Name} ID - {element.Id}");
                Thread.Sleep(500);
            }
        }
        finally
        {
            Console.WriteLine("Release semaphore");
            _semaphoreSlim.Release();
        }
    }

    /// <inheritdoc />
    public override async Task AddElementsAsync(IEnumerable<ProcessElement> elements)
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            Console.WriteLine("Add new elements in semaphore");
            foreach (var element in elements)
            {
                ProcessElements.Enqueue(element);
                Thread.Sleep(500);
            }
        }
        finally
        {
            Console.WriteLine("Release semaphore");
            _semaphoreSlim.Release();
        }
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<ProcessElement>> GetElementsAsync()
    {
        Console.WriteLine("Wait for releasing elements in semaphore");
        await _semaphoreSlim.WaitAsync();
        try
        {
            return ProcessElements;
        }
        finally
        {
            Console.WriteLine("Release semaphore");
            _semaphoreSlim.Release();
        }
    }
}