namespace AsyncTricks.SyncTasks;

/// <summary>
///     Обработчик элементов.
/// </summary>
public abstract class ElementsProcessor
{
    /// <summary>
    ///     Обработанные элементы.
    /// </summary>
    protected readonly List<ProcessElement> ProcessedElements = new();

    /// <summary>
    ///     Очередь на обработку.
    /// </summary>
    protected readonly Queue<ProcessElement> ProcessElements = new();

    /// <summary>
    ///     Обработать элементы
    /// </summary>
    /// <returns>Задача.</returns>
    public abstract Task ProcessElementsAsync();

    /// <summary>
    ///     Добавить элементы для обработки.
    /// </summary>
    /// <param name="elements">Элементы.</param>
    /// <returns>Задача.</returns>
    public abstract Task AddElementsAsync(IEnumerable<ProcessElement> elements);

    /// <summary>
    ///     Получить обработанные элементы.
    /// </summary>
    /// <returns>Элементы.</returns>
    public abstract Task<IEnumerable<ProcessElement>> GetElementsAsync();
}