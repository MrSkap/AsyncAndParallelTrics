namespace AsyncTricks.LongOperations;

public class LongOperationExecutor
{
    public async Task ExecuteAsync()
    {
        var task = new Task(() => AddAndWait(10, 1000), TaskCreationOptions.LongRunning);
        await task;
        await Task.Factory.StartNew(async () => await AddAndWaitAsync(10, 1000), TaskCreationOptions.LongRunning);
    }

    private void AddAndWait(int count, int waitMs)
    {
        var iterationCount = 0;
        while (iterationCount < count)
        {
            Thread.Sleep(waitMs); // do smth
            iterationCount++;
        }
    }

    private async Task AddAndWaitAsync(int count, int waitMs)
    {
        var iterationCount = 0;
        while (iterationCount < count)
        {
            await Task.Delay(waitMs); // do smth async
            iterationCount++;
        }
    }
}