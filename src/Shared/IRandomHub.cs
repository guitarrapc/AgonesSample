using MagicOnion;
using MessagePack;

namespace Shared;

public interface IRandomHub : IStreamingHub<IRandomHub, IRandomHubReciever>
{
    Task<StartResult> StartAsync();
}

public interface IRandomHubReciever
{
    void OnMessageRecieved(string message);
}

[MessagePackObject(true)]
public class StartResult
{
    public string Host { get; }
    public string Id { get; }

    public StartResult(string host, string id)
    {
        Host = host;
        Id = id;
    }
}
