namespace AsyncTricks.Execution;

public class TaskSpammer
{
    public async Task SpamTasksAndWriteWhenTheyComplete(int tasksCount)
    {
        Console.WriteLine("Generate and start tasks");
        var tasks = GenerateTasks(tasksCount);
        var taskForWaiting = TaskRunner.RunAndDoActionByContinueWithAsync(tasks, DoAfterComplete);
        await Task.WhenAll(taskForWaiting);

        Console.WriteLine("\n");
        Console.WriteLine("Do with TaskCompletionSource");
        var tasksWithNoLock = GenerateTasks(tasksCount);
        var tasksForWaitingWithNoLock = TaskRunner.RunAndDoActionAsync(tasksWithNoLock, DoAfterComplete);
        await Task.WhenAll(tasksForWaitingWithNoLock);

        Console.WriteLine("\n");
        Console.WriteLine("Do with Task.WhenAny");
        var tasksWithWhenAny = GenerateTasks(tasksCount);
        var tasksForWaitingWithWhenAny =
            TaskRunner.RunAndDoActionWhenAnyCompleteAsync(tasksWithWhenAny, DoAfterComplete);
        await Task.WhenAll(tasksForWaitingWithWhenAny);
    }

    private IEnumerable<Task<string>> GenerateTasks(int tasksCount)
    {
        var random = new Random();
        var waitTimeForTasks = Enumerable.Range(0, tasksCount).Select(x => random.Next(0, 1000)).ToList();
        Console.WriteLine($"Wait time for tasks: {string.Join("; ", waitTimeForTasks)}");
        foreach (var time in waitTimeForTasks) yield return WaitAndReturnMessage(time);
    }

    private async Task<string> WaitAndReturnMessage(int waitTimeMs)
    {
        await Task.Delay(waitTimeMs);
        return $"I wait {waitTimeMs}";
    }

    private void DoAfterComplete(Task<string> task)
    {
        task.Wait();
        var message = task.Result;
        Console.WriteLine($"Task complete with message: {message}");
    }
}