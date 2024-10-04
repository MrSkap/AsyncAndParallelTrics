using System.Diagnostics;
using AsyncTricks.Calculations;
using AsyncTricks.Cancellation;
using AsyncTricks.Execution;
using AsyncTricks.Loader;
using AsyncTricks.LongOperations;
using AsyncTricks.NotWaitingOperations;

namespace AsyncTricks;

/// <summary>
///     Класс для демонстрации различного использования ассинхронных методов программирования.
/// </summary>
public class ExampleJob
{
    private readonly BigCollectionsSorter _asyncSorter = new();
    private readonly LongOperationsCanceller _longOperationsCanceller = new();
    private readonly SizeOfResponseExtractor _sizeOfResponseExtractor = new();
    private readonly SyncCollectionSorter _syncSorter = new();

    /// <summary>
    ///     Пример для сравнения работы Task.WhenAll Task.WhenAny и синхронного варианта.
    ///     <remarks>
    ///         В данном примере загружаются файлы 3 способами. Синхронно, асинхронно с ожиданием всех Task.WhenAll,
    ///         асинхронно с обработкой результата сразу по завершению операции.
    ///     </remarks>
    /// </summary>
    /// <param name="urls">Урлы для отправки запросов.</param>
    public async Task LoadFilesWithDifferentWaysAsync(List<string> urls)
    {
        Console.WriteLine("Start load one by one");
        var watch = Stopwatch.StartNew();
        await _sizeOfResponseExtractor.LoadOneAfterOne(urls);
        watch.Stop();
        Console.WriteLine($"Complete: {watch.ElapsedMilliseconds}ms");

        Console.WriteLine("Start when all loading");
        watch.Restart();
        await _sizeOfResponseExtractor.LoadWithWhenAll(urls);
        watch.Stop();
        Console.WriteLine($"Complete: {watch.ElapsedMilliseconds}ms");

        Console.WriteLine("Start when any loading");
        watch.Restart();
        await _sizeOfResponseExtractor.LoadWithWhenAny(urls);
        watch.Stop();
        Console.WriteLine($"Complete: {watch.ElapsedMilliseconds}ms");
    }

    /// <summary>
    ///     Пример для демонстрации вычислительной работы с помощью асинхронных методов. Используется параллельный запуск
    ///     нескольких задач созданных с помощью Task.Ran и синхронный вариант.
    ///     <remarks>
    ///         В качестве вычислительной операции используется сортировка массива массивов.
    ///         При простых операциях синхронный вариант может быть быстрее асинхронного. Однако при сложных операциях
    ///         возможность распараллелить и выполнить асинхронно сильно ускоряет время выполнения операции.
    ///     </remarks>
    /// </summary>
    public async Task DoSomeHardCalculatedJobsAsync()
    {
        Console.WriteLine("Use big mount of data for calculating");
        var exampleData = GenerateData(1000, 100000, 1);

        Console.WriteLine("Start sync sort");
        var watch = Stopwatch.StartNew();
        await _syncSorter.SortCollectionsAndCollectionOfCollectionsAsync(exampleData);
        watch.Stop();
        Console.WriteLine($"Complete: {watch.ElapsedMilliseconds}ms");

        exampleData = GenerateData(1000, 100000, 1);
        Console.WriteLine("Start async sort");
        watch.Restart();
        await _asyncSorter.SortCollectionsAndCollectionOfCollectionsAsync(exampleData);
        watch.Stop();
        Console.WriteLine($"Complete: {watch.ElapsedMilliseconds}ms");

        Console.WriteLine("Use small data for calculating");

        exampleData = GenerateData(10000, 10, 1);
        Console.WriteLine("Start sync sort");
        watch.Restart();
        await _syncSorter.SortCollectionsAndCollectionOfCollectionsAsync(exampleData);
        watch.Stop();
        Console.WriteLine($"Complete: {watch.ElapsedMilliseconds}ms");

        exampleData = GenerateData(10000, 10, 1);
        Console.WriteLine("Start async sort");
        watch.Restart();
        await _asyncSorter.SortCollectionsAndCollectionOfCollectionsAsync(exampleData);
        watch.Stop();
        Console.WriteLine($"Complete: {watch.ElapsedMilliseconds}ms");
    }

    public async Task RunSomeTasksAndDoActionsWhenItComplete()
    {
        var spammer = new TaskSpammer();
        await spammer.SpamTasksAndWriteWhenTheyComplete(10);
    }

    public async Task RunOperationsAndCancelThem()
    {
        Console.WriteLine("Run operation and show how it looks like successfully");
        await _longOperationsCanceller.StartOperationsAndCancelIfItTooLongAsync(4, 4500);

        Console.WriteLine();

        Console.WriteLine("Run operation and cancel it after 3 sec of waiting with inner cancellation");
        await _longOperationsCanceller.StartOperationsAndCancelIfItTooLongAsync(7, 3000);

        Console.WriteLine();

        Console.WriteLine("Run operation and cancel it after 3 sec of waiting with external cancellation");
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(3000);
        await _longOperationsCanceller.StartOperationsAndCancelIfItTooLongAsync(7, 5000, cts.Token);
    }

    public async Task RunLongOperationAsync()
    {
        var longOperationExecutor = new LongOperationExecutor();
        await longOperationExecutor.ExecuteAsync();
    }

    private async Task WaitAndThrowAsync(int waitMs, Exception? exceptionToThrow, CancellationToken cancellationToken)
    {
        await Task.Delay(waitMs, cancellationToken);
        if (exceptionToThrow is not null) throw exceptionToThrow;
    }

    private static List<List<int>> GenerateData(int headListSize, int innerListSize, int seed)
    {
        var random = new Random(seed);

        return Enumerable
            .Range(0, headListSize)
            .Select(i =>
                Enumerable
                    .Range(0, innerListSize)
                    .Select(j =>
                        random.Next())
                    .ToList())
            .ToList();
    }
}