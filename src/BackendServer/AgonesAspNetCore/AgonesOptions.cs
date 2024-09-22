namespace AgonesAspNetCore;

public class AgonesOptions
{
    /// <summary>
    /// CancellationTokenSource to manage AgonesSDK and AgonesHealthKeeper
    /// </summary>
    public CancellationTokenSource SdkCancellationTokenSource { get; } = new CancellationTokenSource();
    /// <summary>
    /// AgonesSDK Emulator Fleet Name
    /// </summary>
    public string EmulateSdkFleetName { get; set; } = "dummyfleet";
    /// <summary>
    /// AgonesSDK Emulator Return GameServer Namespace
    /// </summary>
    public string EmulateSdkNameSpace { get; set; } = "default";
    /// <summary>
    /// AgonesSDK Emulator Return GameServer port
    /// </summary>
    public int EmulateSdkPort { get; set; } = 80;
    /// <summary>
    /// Healthcheck Initial Delay period to begin.
    /// </summary>
    public TimeSpan HealthcheckDelay { get; set; } = TimeSpan.FromSeconds(5);
    /// <summary>
    /// Healthcheck interval period.
    /// </summary>
    public TimeSpan HealthcheckInterval { get; set; } = TimeSpan.FromSeconds(5);
    /// <summary>
    /// Healthcheck Failure Threshold to consider unhealthy.
    /// </summary>
    public int HealthcheckFailureThreshold { get; set; } = 3;
}
