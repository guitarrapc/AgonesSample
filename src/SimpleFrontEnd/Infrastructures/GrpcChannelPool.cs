using Grpc.Net.Client;
using System.Collections.Concurrent;

namespace SimpleFrontEnd.Infrastructures;

public class GrpcChannelPool
{
    private readonly ConcurrentDictionary<string, GrpcChannel> _channels = new();

    public static GrpcChannelPool Instance { get; } = new GrpcChannelPool();

    public GrpcChannel CreateChannel(string host)
    {
        var channel = _channels.GetValueOrDefault(host, GrpcChannel.ForAddress(host));
        switch (channel.State)
        {
            case Grpc.Core.ConnectivityState.Ready:
                return channel;
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
        };
    }
}
