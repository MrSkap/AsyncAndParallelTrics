namespace AsyncTricks.SyncTasks;

public abstract class ElementsProcessor
{
    protected Queue<ProcessElement> SyncProcessElements = new ();
    protected Queue<ProcessElement> ProcessElements = new();
    protected List<ProcessElement> ProcessedElements = new();
    
    public abstract Task ProcessElementsAsync();
    public abstract Task AddElementsAsync();
    public abstract Task GetElementsAsync();
}