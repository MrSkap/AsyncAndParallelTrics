namespace AsyncTricks.NotWaitingOperations;

public static class TaskExtensions
{
    /// <summary>
    ///     Не ожидать завершения задачи. Вывести ошибку в консоль и продолжить работу.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public static void DoNotWait(this Task task, CancellationToken cancellationToken)
    {
        task.ContinueWith(
            x => Console.Error.WriteLine($"Catch exception while executing not awaitable task. \n {x.Exception}"),
            cancellationToken,
            TaskContinuationOptions.OnlyOnFaulted,
            TaskScheduler.Default);
    }

    /// <summary>
    ///     Не ожидать завершения задачи. Упасть в случае ошибки.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public static void DoNotWaitAndFallOnException(this Task task, CancellationToken cancellationToken)
    {
        task.ContinueWith(
            x =>
            {
                Console.Error.WriteLine($"Catch fatal exception while executing not awaitable task. \n {x.Exception}");
                Environment.Exit(1);
            },
            cancellationToken,
            TaskContinuationOptions.OnlyOnFaulted,
            TaskScheduler.Default);
    }
}