namespace Shared;

public class KubernetesServiceProvider
{
    public static KubernetesServiceProvider Current = new KubernetesServiceProvider();

    private bool? _isRunningOnKubernetes;
    private bool? _isRunningOnDocker;
    private string? _namespace;
    private string? _hostName;
    private string? _accessToken;
    private string? _kubernetesServiceEndPoint;

    public bool IsRunningOnDocker
        => _isRunningOnDocker ?? (bool)(_isRunningOnDocker = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")));
    public bool IsRunningOnKubernetes
        => _isRunningOnKubernetes ?? (bool)(_isRunningOnKubernetes = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST")));
    public string AccessToken
        => _accessToken ??= IsRunningOnKubernetes ? File.ReadAllText("/var/run/secrets/kubernetes.io/serviceaccount/token") : "";
    public string HostName
        => _hostName ??= Environment.GetEnvironmentVariable("HOSTNAME")!;
    public string KubernetesServiceEndPoint
        => _kubernetesServiceEndPoint ??= $"https://{Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST")}:{Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_PORT")}";
    public string Namespace
        => _namespace ??= IsRunningOnKubernetes ? File.ReadAllText("/var/run/secrets/kubernetes.io/serviceaccount/namespace") : "";
}
