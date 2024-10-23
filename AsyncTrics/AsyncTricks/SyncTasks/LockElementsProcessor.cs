namespace AsyncTricks.SyncTasks;

/// <summary>
///     Синхронный обработчик эелментов через lock.
/// </summary>
public class LockElementsProcessor : ElementsProcessor
{
    private readonly object _lockObj = new();

    /// <inheritdoc />
    public override Task ProcessElementsAsync()
    {
        Console.WriteLine("Process elements in lock");
        lock (_lockObj)
        {
            while (ProcessElements.TryDequeue(out var element))
            {
                ProcessedElements.Add(element);
                Console.WriteLine($"Name - {element.Name} ID - {element.Id}");
                Thread.Sleep(500);
            }

            Console.WriteLine("End of lock");
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task AddElementsAsync(IEnumerable<ProcessElement> elements)
    {
        lock (_lockObj)
        {
            Console.WriteLine("Add new elements in lock");
            foreach (var element in elements)
            {
                ProcessElements.Enqueue(element);
                Thread.Sleep(500);
            }

            Console.WriteLine("End of lock");
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task<IEnumerable<ProcessElement>> GetElementsAsync()
    {
        Console.WriteLine("Wait for releasing elements in lock");
        lock (_lockObj)
        {
            Console.WriteLine("End of lock");
            return Task.FromResult<IEnumerable<ProcessElement>>(ProcessedElements);
        }
    }
}