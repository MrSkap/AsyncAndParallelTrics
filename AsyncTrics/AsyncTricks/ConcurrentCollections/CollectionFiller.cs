using System.Collections.Concurrent;

namespace AsyncTricks.ConcurrentCollections;

/// <summary>
///     Получение конкурентных коллекций.
/// </summary>
/// <param name="maxSize">Максимальный размер коллекции.</param>
/// <param name="addIterations"> Количество попыток добавления элементов.</param>
/// <remarks>
///     Все элементы добавляются асинхронно, поэтому итоговое количество может быть больше заданного! Если попыток
///     больше чем размер коллекции, то один из элементов будет заменен новым.
/// </remarks>
public class CollectionFiller(int maxSize, int addIterations)
{
    private readonly string[] _names =
    {
        "some_0", "some_1", "some_2", "some_3", "some_4", "some_5", "some_6", "some_7", "some_8", "some_9"
    };

    public async Task<ConcurrentDictionary<string, int>> GetDictionaryAsync()
    {
        var dictionary = new ConcurrentDictionary<string, int>();
        var tasks = Enumerable.Range(0, addIterations)
            .Select(i => Task.Run(() => AddOrChangeItem(dictionary)));

        await Task.WhenAll(tasks);
        return dictionary;
    }

    public async Task<ConcurrentQueue<int>> GetQueueAsync()
    {
        var queue = new ConcurrentQueue<int>();
        var random = new Random();
        var tasks = Enumerable.Range(0, addIterations).Select(i => Task.Run(() => AddInQueue(maxSize, queue, random)));

        await Task.WhenAll(tasks);
        return queue;
    }

    public async Task<ConcurrentBag<int>> GetBagAsync()
    {
        var bag = new ConcurrentBag<int>();
        var random = new Random();
        var tasks = Enumerable.Range(0, addIterations).Select(i => Task.Run(() => AddInBag(maxSize, bag, random)));

        await Task.WhenAll(tasks);
        return bag;
    }

    public async Task<ConcurrentStack<int>> GetStackAsync()
    {
        var stack = new ConcurrentStack<int>();
        var random = new Random();
        var tasks = Enumerable.Range(0, addIterations).Select(i => Task.Run(() => AddInStack(maxSize, stack, random)));

        await Task.WhenAll(tasks);
        return stack;
    }

    private static void AddInQueue(int maxLength, ConcurrentQueue<int> queue, Random random)
    {
        if (queue.Count < maxLength)
        {
            queue.Enqueue(random.Next(0, 100));
        }
        else
        {
            if (!queue.TryDequeue(out var removed)) return;
            var next = random.Next(0, 100);
            Console.WriteLine($"Dequeue first element {removed} and enqueue {next}");
            queue.Enqueue(next);
        }
    }

    private static void AddInStack(int maxLength, ConcurrentStack<int> stack, Random random)
    {
        if (stack.Count < maxLength)
        {
            stack.Push(random.Next(0, 100));
        }
        else
        {
            if (!stack.TryPop(out var removed)) return;
            var next = random.Next(0, 100);
            Console.WriteLine($"Pop top element {removed} and push {next}");
            stack.Push(next);
        }
    }

    private static void AddInBag(int maxLength, ConcurrentBag<int> bag, Random random)
    {
        if (bag.Count < maxLength)
        {
            bag.Add(random.Next(0, 100));
        }
        else
        {
            if (!bag.TryTake(out var removed)) return;
            var next = random.Next(0, 100);
            Console.WriteLine($"Take first element {removed} and add {next}");
            bag.Add(next);
        }
    }

    private void AddOrChangeItem(ConcurrentDictionary<string, int> dictionary)
    {
        var random = new Random();
        var nameIndex = random.Next(0, _names.Length);
        var value = random.Next(0, 100);

        dictionary.AddOrUpdate(_names[nameIndex], value, (key, oldValue) =>
        {
            Console.WriteLine($"Update {key} value from {oldValue} to {value}");
            return value;
        });
    }
}