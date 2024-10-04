// See https://aka.ms/new-console-template for more information

using AsyncTricks;

// лучше использовать более долгие запросы
var urls = new List<string>
{
    "https://catfact.ninja/fact",
    "https://official-joke-api.appspot.com/random_joke",
    "https://dog.ceo/api/breeds/image/random"
};
var example = new ExampleJob();

// Пример использования Task.WhenAll() и Task.WhenAny
// + сравнение скорости выполнения синхронного и асинхронного варианта выполнения
await example.LoadFilesWithDifferentWaysAsync(urls);

Console.WriteLine("\n \n");

// Пример использовая Task.Run() и распараллеливания вычислительных задач
// + сравнение скорости выполнения синхронного и асинхронного варианта выполнения
await example.DoSomeHardCalculatedJobsAsync();

Console.WriteLine("\n \n");

// Пример различных способов ожидания задач
await example.RunSomeTasksAndDoActionsWhenItComplete();

Console.WriteLine("\n \n");

// Пример отмены операции
await example.RunOperationsAndCancelThem();

// Пример долговыполняемой операции
await example.RunLongOperationAsync();

// Пример запуска аснихронной операции без ожидания
// и обработка исключения при ее завершении с ошибкой
await example.RunNotAwaitableTaskAndFailIt();

// Пример использования IAsyncEnumerable
await example.ShowAsyncEnumerableProfitAsync();