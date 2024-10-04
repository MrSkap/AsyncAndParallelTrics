using System.Runtime.CompilerServices;

namespace AsyncTricks.AsyncIterator;

public class DataGenerator
{
    private readonly Random _random = new();

    public async IAsyncEnumerable<Foo> GetEnumerableFoosWithDelayAsync(int count, int delay, int min = 0, int max = 100,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        for (var i = 0; i < count; i++)
        {
            if (cancellationToken.IsCancellationRequested) yield break;

            await Task.Delay(delay, cancellationToken);

            var number1 = _random.Next(min, max);
            var number2 = _random.Next(min, max);
            var compareResult = GetCompareSymbolResult(number1, number2);

            yield return new Foo
            {
                Number1 = number1,
                Number2 = number2,
                Line = $"{number1} {compareResult} {number2}"
            };
        }
    }

    public async Task<List<Foo>> GetFoosWithDelayAsync(int count, int delay, int min = 0, int max = 100,
        CancellationToken cancellationToken = default)
    {
        var result = new List<Foo>();
        for (var i = 0; i < count; i++)
        {
            if (cancellationToken.IsCancellationRequested) break;

            await Task.Delay(delay, cancellationToken);

            var number1 = _random.Next(min, max);
            var number2 = _random.Next(min, max);
            var compareResult = GetCompareSymbolResult(number1, number2);

            result.Add(new Foo
            {
                Number1 = number1,
                Number2 = number2,
                Line = $"{number1} {compareResult} {number2}"
            });
        }

        return result;
    }

    public IEnumerable<Foo> GetFoos(int count, int min = 0, int max = 100)
    {
        var result = new List<Foo>();
        for (var i = 0; i < count; i++)
        {
            var number1 = _random.Next(min, max);
            var number2 = _random.Next(min, max);
            var compareResult = GetCompareSymbolResult(number1, number2);

            yield return new Foo
            {
                Number1 = number1,
                Number2 = number2,
                Line = $"{number1} {compareResult} {number2}"
            };
        }
    }


    private static string GetCompareSymbolResult(int a, int b)
    {
        return a > b
            ? ">"
            : a < b
                ? "<"
                : "==";
    }
}