// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using AsyncTricks.Loader;

// лучше использовать более долгие запросы
var urls = new List<string>
{
    "https://catfact.ninja/fact",
    "https://official-joke-api.appspot.com/random_joke",
    "https://dog.ceo/api/breeds/image/random"
};

var fileLoader = new SizeOfResponseExtractor();

Console.WriteLine("Start load one by one");
var watch = Stopwatch.StartNew();
await fileLoader.LoadOneAfterOne(urls);
watch.Stop();
Console.WriteLine($"Complete: {watch.ElapsedMilliseconds}ms");

Console.WriteLine("Start when all loading");
watch = Stopwatch.StartNew();
await fileLoader.LoadWithWhenAll(urls);
watch.Stop();
Console.WriteLine($"Complete: {watch.ElapsedMilliseconds}ms");

Console.WriteLine("Start when any loading");
watch = Stopwatch.StartNew();
await fileLoader.LoadWithWhenAny(urls);
watch.Stop();
Console.WriteLine($"Complete: {watch.ElapsedMilliseconds}ms");