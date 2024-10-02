namespace AsyncTricks.Execution;

public static class TaskRunner
{
    public static List<Task<T>> RunAndDoActionByContinueWithAsync<T>(IEnumerable<Task<T>> tasks, Action<Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        var inputTasks = tasks.ToList();

        foreach (var task in inputTasks)
        {
            // can cause lock
            task.ContinueWith(action, cancellationToken, TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }

        return inputTasks;
    }

    public static Task<Task<T>>[] RunAndDoActionAsync<T>(IEnumerable<Task<T>> tasks, Action<Task<T>> action)
    {
        var inputTasks = tasks.ToList();

        var buckets = new TaskCompletionSource<Task<T>>[inputTasks.Count];
        var results = new Task<Task<T>>[buckets.Length];
        for (var i = 0; i < buckets.Length; i++)
        {
            buckets[i] = new TaskCompletionSource<Task<T>>();
            results[i] = buckets[i].Task;
        }

        var nextTaskIndex = -1;
        Action<Task<T>> continuation = completed =>
        {
            var bucket = buckets[Interlocked.Increment(ref nextTaskIndex)];
            bucket.TrySetResult(completed);
            action.Invoke(completed);
        };

        foreach (var inputTask in inputTasks)
        {
            inputTask.ContinueWith(continuation, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }

        return results;
    }

    public static async Task<List<Task<T>>> RunAndDoActionWhenAnyCompleteAsync<T>(IEnumerable<Task<T>> tasks, Action<Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        var activeTasks = tasks.ToList();
        while (activeTasks.Count != 0 && cancellationToken.IsCancellationRequested is false)
        {
            var completed = await Task.WhenAny(activeTasks);
            activeTasks.Remove(completed);
            action.Invoke(completed);
        }

        return activeTasks;
    }
}