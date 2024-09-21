if (args is { Length: > 0 })
{
    var serverTask = args[0].ToLowerInvariant() switch
    {
        "api" => SimpleBackendApi.Program.Main(Array.Empty<string>()),
        "grpc" => SimpleBackendGrpc.Program.Main(Array.Empty<string>()),
        _ => throw new InvalidOperationException($"{args[0]} is unknown target.")
    };
    await serverTask;
}
else
{
    var serverGrpcTask = SimpleBackendGrpc.Program.Main(Array.Empty<string>());
    var serverApiTask = SimpleBackendApi.Program.Main(Array.Empty<string>());
    await Task.WhenAll(serverGrpcTask, serverApiTask);
}
