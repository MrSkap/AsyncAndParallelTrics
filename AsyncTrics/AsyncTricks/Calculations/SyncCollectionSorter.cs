namespace AsyncTricks.Calculations;

public class SyncCollectionSorter : ISorter
{
    public Task<List<List<int>>> SortCollectionsAndCollectionOfCollectionsAsync(List<List<int>> listOfLists)
    {
        SortInnerCollections(listOfLists);
        listOfLists.Sort(new ListOfListsComparer());
        return Task.FromResult(listOfLists);
    }

    private static void SortInnerCollections(List<List<int>> listOfLists)
    {
        listOfLists.ForEach(list => list.Sort());
    }
}