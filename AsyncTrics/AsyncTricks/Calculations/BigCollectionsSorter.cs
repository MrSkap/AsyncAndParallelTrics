namespace AsyncTricks.Calculations;

public class BigCollectionsSorter : ISorter
{
    public async Task<List<List<int>>> SortCollectionsAndCollectionOfCollectionsAsync(List<List<int>> listOfLists)
    {
        await SortInnerCollectionsAsync(listOfLists);
        await SortListOfLists(listOfLists);
        return listOfLists;
    }

    private static async Task SortInnerCollectionsAsync(List<List<int>> listOfLists)
    {
        var innerSortTasks = listOfLists.Select(async collection => { await Task.Run(collection.Sort); }).ToList();
        await Task.WhenAll(innerSortTasks);
    }

    private static async Task SortListOfLists(List<List<int>> listOfLists)
    {
        await Task.Run(() => { listOfLists.Sort(new ListOfListsComparer()); });
    }
}