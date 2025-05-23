using MagicOnion;
using MessagePack;

namespace Shared;

public interface IRandomHub : IStreamingHub<IRandomHub, IRandomHubReciever>
{
    Task JoinAsync(JoinRequest request);
    Task<StartResult> StartAsync();
}

public interface IRandomHubReciever
{
    void OnJoin(string userName);
    void OnMessageRecieved(string message);
}

[MessagePackObject(true)]
public record StartResult
{
    public required string Host { get; init; }
    public required Guid Id { get; init; }
}

[MessagePackObject(true)]
public record JoinRequest
{
    public required string RoomName { get; set; }
    public required string UserName { get; set; }
}
