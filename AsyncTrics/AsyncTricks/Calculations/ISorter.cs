namespace AsyncTricks.Calculations;

public interface ISorter
{
    public Task<List<List<int>>> SortCollectionsAndCollectionOfCollectionsAsync(List<List<int>> listOfLists);
}