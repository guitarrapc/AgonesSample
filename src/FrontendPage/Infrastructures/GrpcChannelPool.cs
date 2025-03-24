using Grpc.Net.Client;
using System.Collections.Concurrent;

namespace FrontendPage.Infrastructures;

public class GrpcChannelPool : IDisposable
{
    private readonly ConcurrentDictionary<string, GrpcChannel> _channels = new();

    public static GrpcChannelPool Instance { get; } = new GrpcChannelPool();

    public GrpcChannel CreateChannel(string host)
    {
        var channel = _channels.GetValueOrDefault(host, GrpcChannel.ForAddress(host));
        switch (channel.State)
        {
            case Grpc.Core.ConnectivityState.TransientFailure:
                _channels.TryRemove(host, out _);
                channel.Dispose();
                return CreateChannel(host);
            case Grpc.Core.ConnectivityState.Shutdown:
                _channels.TryRemove(host, out _);
                channel.Dispose();
                return CreateChannel(host);
            default:
                return channel;
        }
        ;
    }

    public void Dispose()
    {
        foreach (var channel in _channels.Values)
        {
            channel.Dispose();
        }
    }
}
