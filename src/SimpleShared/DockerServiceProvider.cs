namespace SimpleShared;

public class DockerServiceProvider
{
    public static DockerServiceProvider Current = new DockerServiceProvider();

    private bool? _isRunningOnDocker;

    public bool IsRunningOnDocker
        => _isRunningOnDocker ?? (bool)(_isRunningOnDocker = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")));
}
