
namespace SimpleFrontEnd.Data;

public interface IAgonesAllocationDatabase
{
    int Count { get; }
    IReadOnlySet<string> Items { get; }

    void Add(string item);
    void Clear();
    bool Remove(string item);
    void Replace(IReadOnlySet<string> replaceTo);
}
