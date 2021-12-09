namespace SimpleFrontEnd.Data;

public class InmemoryAgonesAllocationDatabase : IAgonesAllocationDatabase
{
    public IReadOnlySet<string> Items => _items;
    private readonly SortedSet<string> _items = new();

    public int Count => _items.Count;
    public void Add(string item) => _items.Add(item);
    public void Clear() => _items.Clear();
    public bool Remove(string item) => _items.Remove(item);

    /// <summary>
    /// Clear all existing and replace with new items.
    /// </summary>
    /// <param name="replaceTo"></param>
    public void Replace(IReadOnlySet<string> replaceTo)
    {
        Clear();
        foreach (var item in replaceTo)
        {
            Add(item);
        }
    }
}

public class AgonesAllocation
{
    public string? Host { get; set; }
    public int? Port { get; set; }
}
