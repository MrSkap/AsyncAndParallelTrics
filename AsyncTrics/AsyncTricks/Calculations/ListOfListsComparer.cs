namespace AsyncTricks.Calculations;

public class ListOfListsComparer : IComparer<List<int>>
{
    public int Compare(List<int>? x, List<int>? y)
    {
        if (x == y) return 0;

        if (x is null && y is null) return 0;

        if (x is null) return -1;

        if (y is null) return 1;

        if (x.Count == y.Count) return 0;

        return x.Count > y.Count
            ? 1
            : -1;
    }
}