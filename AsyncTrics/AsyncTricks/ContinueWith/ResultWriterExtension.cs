namespace AsyncTricks.ContinueWith;

public static class ResultWriterExtension
{
    /// <summary>
    ///     Вывести результат задачи в консоль.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <typeparam name="T">Результат задачи.</typeparam>
    /// <returns>Задача.</returns>
    public static Task<T> WriteResultAsync<T>(this Task<T> task)
    {
        task.ContinueWith(async resultTask =>
        {
            var res = await resultTask;
            Console.WriteLine($"Result of task: {res?.ToString()}");
            return resultTask;
        });
        return task;
    }

    public static Task<T> DoAfterIfSuccess<T>(this Task<T> task, Func<T, bool> predicate)
    {
        task.ContinueWith(async mainTask =>
        {
            T res;
            try
            {
                res = await mainTask;
            }
            catch (Exception)
            {
                Console.WriteLine("Cant run next task because main task failed");
                throw;
            }

            predicate(res);
        });
        return task;
    }

    /// <summary>
    ///     Выполнить действие после задачи.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <param name="action">Действие.</param>
    /// <typeparam name="T">Результат задачи.</typeparam>
    /// <returns>Задача.</returns>
    public static Task<T> DoAfter<T>(this Task<T> task, Action<T> action)
    {
        task.ContinueWith(async resultTask => action(await resultTask));
        return task;
    }

    /// <summary>
    ///     Выполнить действие после задачи.
    /// </summary>
    /// <param name="task">Задача.</param>
    /// <param name="action">Действие.</param>
    /// <typeparam name="T">Результат задачи.</typeparam>
    /// <returns>Задача.</returns>
    public static async Task<T> DoAfter<T>(this Task<T> task, Func<T, Task<T>> action)
    {
        return await await task.ContinueWith(async resultTask => await action(await resultTask));
    }
}