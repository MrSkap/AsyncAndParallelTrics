namespace AsyncTricks.Cancellation;

public class LongOperationsCanceller
{
    /// <summary>
    /// Выполнить операцию. Успешно выполняется, если все попытки будут завершены до отмены операции.
    /// </summary>
    /// <remarks>1 попытка - 0.5 секунды | 500 милисекунд.</remarks>
    /// <param name="retryCount">Количество попыток.</param>
    /// <param name="waitMs">Ожидать до отмены операции.</param>
    /// <param name="cancellationToken">Токен для преждевременной отмены.</param>
    public async Task StartOperationsAndCancelIfItTooLongAsync(int retryCount, int waitMs, CancellationToken cancellationToken = default)
    {
        var someCancellationTokenSource = new CancellationTokenSource();
        someCancellationTokenSource.CancelAfter(waitMs);
        try
        {
            await LongOperationAsync(retryCount, someCancellationTokenSource.Token, cancellationToken);
        }
        catch (OperationCanceledException ex)
        {
            Console.WriteLine($"Operation was cancelled. Cancellation error: {ex.Message}");
        }
        finally
        {
            if (someCancellationTokenSource.IsCancellationRequested is false && cancellationToken.IsCancellationRequested is false)
            {
                Console.WriteLine("Operation completed successfully");
            }
            someCancellationTokenSource.Dispose();
        }
    }
    
    private static async Task LongOperationAsync(int maxRetryCount, CancellationToken innerCancellationToken, CancellationToken externalCancellationToken)
    {
        var currentRetryCount = 0;
        while (currentRetryCount < maxRetryCount 
               && innerCancellationToken.IsCancellationRequested is false 
               && externalCancellationToken.IsCancellationRequested is false)
        {
            Console.WriteLine("Do some work...");
            await Task.Delay(500, innerCancellationToken);
            currentRetryCount++;
        }

        if (innerCancellationToken.IsCancellationRequested || externalCancellationToken.IsCancellationRequested)
        {
            throw new OperationCanceledException();
        }
    }
}