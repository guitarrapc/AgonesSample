namespace AgonesAspNetCore;

public class AgonesOptions
{
    /// <summary>
    /// CancellationTokenSource to manage AgonesSDK and AgonesHealthKeeper
    /// </summary>
    public CancellationTokenSource SdkCancellationTokenSource { get; } = new CancellationTokenSource();
    /// <summary>
    /// AgonesSDK Emulator Return port
    /// </summary>
    public int EmulatedSdkPort { get; set; }
    /// <summary>
    /// Healthcheck Initial Delay period to begin.
    /// </summary>
    public TimeSpan HealthcheckDelay { get; set; } = TimeSpan.FromSeconds(5);
    /// <summary>
    /// Healthcheck interval period.
    /// </summary>
    public TimeSpan HealthcheckInterval { get; set; } = TimeSpan.FromSeconds(5);
}
