using System.Collections;

namespace SimpleFrontEnd.Data;
public class AgonesAllocationSet
{
    public IReadOnlySet<string> Items => _items;
    private readonly SortedSet<string> _items = new();

    public int Length => _items.Count;
    public void Add(AgonesAllocation item) => _items.Add($"{item.Host}:{item.Port}");
    public void Add(string item) => _items.Add(item);
    public void Clear() => _items.Clear();
    public bool Remove(string item) => _items.Remove(item);

    /// <summary>
    /// Get Address Random or nothing.
    /// </summary>
    /// <returns></returns>
    public string[] GetAll()
    {
        if (_items.Count == 0)
            return Array.Empty<string>();

        return _items.Select(x => $"http://{x}").ToArray();
    }
    /// <summary>
    /// Get Address Random or nothing.
    /// </summary>
    /// <returns></returns>
    public string? GetAddressRandomOrDefault()
    {
        if (_items.Count == 0)
            return null;

        var item = _items.Skip(Random.Shared.Next(0, _items.Count - 1)).First();
        return $"http://{item}";
    }
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
