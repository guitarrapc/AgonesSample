if (args is { Length: > 0 })
{
    var serverTask = args[0].ToLowerInvariant() switch
    {
        "api" => SimpleBackendApi.Program.RunApp(Array.Empty<string>()),
        "grpc" => SimpleBackendGrpc.Program.RunApp(Array.Empty<string>()),
        _ => throw new InvalidOperationException($"{args[0]} is unknown target.")
    };
    await serverTask;
}
else
{
    var serverGrpcTask = SimpleBackendGrpc.Program.RunApp(Array.Empty<string>());
    var serverApiTask = SimpleBackendApi.Program.RunApp(Array.Empty<string>());
    await Task.WhenAll(serverGrpcTask, serverApiTask);
}
