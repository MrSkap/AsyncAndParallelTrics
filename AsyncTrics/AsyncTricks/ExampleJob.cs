using System.Diagnostics;
using AsyncTricks.AsyncIterator;
using AsyncTricks.Calculations;
using AsyncTricks.Cancellation;
using AsyncTricks.ConcurrentCollections;
using AsyncTricks.Execution;
using AsyncTricks.Loader;
using AsyncTricks.LongOperations;
using AsyncTricks.NotWaitingOperations;
using AsyncTricks.SyncTasks;

namespace AsyncTricks;

/// <summary>
///     Класс для демонстрации различного использования ассинхронных методов программирования.
/// </summary>
public class ExampleJob
{
    private readonly BigCollectionsSorter _asyncSorter = new();
    private readonly CollectionFiller _collectionFiller = new(10, 30);
    private readonly CompareWriter _compareWriter = new();
    private readonly ElementsProcessor _lockProcessor = new LockElementsProcessor();
    private readonly LongOperationsCanceller _longOperationsCanceller = new();
    private readonly ElementsProcessor _semaphoreProcessor = new SemaphoreElementsProcessor();
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

    public async Task RunNotAwaitableTaskAndFailIt()
    {
        using var cts = new CancellationTokenSource();
        Console.WriteLine("Start failing operation");
        WaitAndThrowAsync(1000, new Exception(), cts.Token).DoNotWait(cts.Token);
        await Task.Delay(1200, cts.Token);
        Console.WriteLine("End of method that call failed operation");
    }

    public async Task ShowAsyncEnumerableProfitAsync()
    {
        Console.WriteLine("Start sync generation and comparing");
        var watch = Stopwatch.StartNew();
        await _compareWriter.GeneratePairsAndCompareAllThanWriteResult(5, 1000, 0, 100, CancellationToken.None);
        watch.Stop();
        Console.WriteLine($"Complete: {watch.ElapsedMilliseconds}ms");

        Console.WriteLine();
        Console.WriteLine("Start async generation and sync comparing");
        watch.Restart();
        await _compareWriter.GeneratePairsAndCompareViaForeachAsync(5, 1000, 0, 100, CancellationToken.None);
        watch.Stop();
        Console.WriteLine($"Complete: {watch.ElapsedMilliseconds}ms");

        Console.WriteLine();
        Console.WriteLine("Start async generation and parallel comparing");
        watch.Restart();
        await _compareWriter.GeneratePairsAndCompareViaParallelForeachAsync(5, 1000, 0, 100, CancellationToken.None);
        watch.Stop();
        Console.WriteLine($"Complete: {watch.ElapsedMilliseconds}ms");
    }

    public async Task CreateAndFillConcurrentCollections()
    {
        Console.WriteLine("Get Concurrent Dictionary");
        var dictionary = await _collectionFiller.GetDictionaryAsync();
        Console.WriteLine("Dictionary is \n");
        dictionary.ToList().ForEach(x => Console.WriteLine($"{x.Key} - {x.Value}"));

        Console.WriteLine();

        Console.WriteLine("Get Concurrent Queue");
        var queue = await _collectionFiller.GetQueueAsync();
        var res = string.Empty;
        while (queue.TryDequeue(out var elem)) res += $" {elem}";
        Console.WriteLine($"Queue is \n {res}");

        Console.WriteLine();

        Console.WriteLine("Get Concurrent Bag");
        var bag = await _collectionFiller.GetBagAsync();
        res = string.Empty;
        while (bag.TryTake(out var elem)) res += $" {elem}";
        Console.WriteLine($"Bag is \n {res}");

        Console.WriteLine();

        Console.WriteLine("Get Concurrent Stack");
        var stack = await _collectionFiller.GetStackAsync();
        res = string.Empty;
        while (stack.TryPop(out var elem)) res += $" {elem}";
        Console.WriteLine($"Stack is \n {res}");
    }

    public async Task CreateAndSyncProcessElementsAsync()
    {
        var elements = GenerateElements(10);

        Console.WriteLine("Start process elements with lock synchronizer");
        var addLockTask = _lockProcessor.AddElementsAsync(elements);
        var processLockTask = _lockProcessor.ProcessElementsAsync();
        var getLockTask = _lockProcessor.GetElementsAsync();

        await Task.WhenAll(addLockTask, processLockTask, getLockTask);

        Console.WriteLine("Start process elements with semaphore synchronizer");
        var addSemTask = _semaphoreProcessor.AddElementsAsync(elements);
        var processSemTask = _semaphoreProcessor.ProcessElementsAsync();
        var getSemTask = _semaphoreProcessor.GetElementsAsync();

        await Task.WhenAll(addSemTask, processSemTask, getSemTask);
    }

    private List<ProcessElement> GenerateElements(int count)
    {
        return Enumerable.Range(0, count)
            .Select(x => new ProcessElement
            {
                Id = Guid.NewGuid(),
                Name = $"Element {x}"
            }).ToList();
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