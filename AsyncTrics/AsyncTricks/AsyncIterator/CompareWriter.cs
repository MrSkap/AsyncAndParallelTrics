using System.Collections.Concurrent;

namespace AsyncTricks.AsyncIterator;

public class CompareWriter
{
    private readonly DataGenerator _dataGenerator = new();

    /// <summary>
    ///     Получить аснихронный поток пар для сравнения и обработать их асинхронно.
    /// </summary>
    /// <param name="pairsCount">Кол-во пар.</param>
    /// <param name="delay">задержка перед получением новой пары.</param>
    /// <param name="min">Минимальное значение.</param>
    /// <param name="max">Максимальное значение.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public async Task GeneratePairsAndCompareViaForeachAsync(int pairsCount, int delay, int min, int max,
        CancellationToken cancellationToken)
    {
        var foos = _dataGenerator.GetEnumerableFoosWithDelayAsync(pairsCount, delay, min, max, cancellationToken);

        Console.WriteLine("Start process of comparing pairs");

        var bag = new ConcurrentBag<string>();
        await foreach (var pair in foos)
            bag.Add($"Compare {pair.Number1} and {pair.Number2} \n Result: {pair.Line}");
        WriteResults(bag);
    }

    /// <summary>
    ///     Получить все пары для сравнения и обработать синхронно.
    /// </summary>
    /// <param name="pairsCount">Кол-во пар.</param>
    /// <param name="delay">задержка перед получением новой пары.</param>
    /// <param name="min">Минимальное значение.</param>
    /// <param name="max">Максимальное значение.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public async Task GeneratePairsAndCompareAllThanWriteResult(int pairsCount, int delay, int min, int max,
        CancellationToken cancellationToken)
    {
        var foos = await _dataGenerator.GetFoosWithDelayAsync(pairsCount, delay, min, max, cancellationToken);

        Console.WriteLine("Start process of comparing pairs");

        var bag = new ConcurrentBag<string>();
        foreach (var pair in foos)
            bag.Add($"Compare {pair.Number1} and {pair.Number2} \n Result: {pair.Line}");
        WriteResults(bag);
    }

    /// <summary>
    ///     Получить аснихронный поток пар для сравнения и обработать их параллельно.
    /// </summary>
    /// <param name="pairsCount">Кол-во пар.</param>
    /// <param name="delay">задержка перед получением новой пары.</param>
    /// <param name="min">Минимальное значение.</param>
    /// <param name="max">Максимальное значение.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public async Task GeneratePairsAndCompareViaParallelForeachAsync(int pairsCount, int delay, int min, int max,
        CancellationToken cancellationToken)
    {
        var foos = _dataGenerator.GetEnumerableFoosWithDelayAsync(pairsCount, delay, min, max, cancellationToken);

        Console.WriteLine("Start process of comparing pairs");

        var options = new ParallelOptions
        {
            CancellationToken = cancellationToken,
            TaskScheduler = TaskScheduler.Default,
            MaxDegreeOfParallelism = 20
        };
        var bag = new ConcurrentBag<string>();
        await Parallel.ForEachAsync(foos, options, (pair, _) =>
        {
            bag.Add($"Compare {pair.Number1} and {pair.Number2} \n Result: {pair.Line}");
            return ValueTask.CompletedTask;
        });
        WriteResults(bag);
    }

    private void WriteResults(IEnumerable<string> results)
    {
        foreach (var result in results) Console.WriteLine(result);
    }
}