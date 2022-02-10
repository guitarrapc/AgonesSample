namespace AgonesAspNetCore;

public class AgonesOptions
{
    /// <summary>
    /// CancellationTokenSource to manage AgonesSDK and AgonesHealthKeeper
    /// </summary>
    public CancellationTokenSource SdkCancellationTokenSource { get; } = new CancellationTokenSource();
    /// <summary>
    /// AgonesSDK Emulator Return GameServer Name
    /// </summary>
    public string EmulateSdkName { get; set; } = "DummyGameServer";
    /// <summary>
    /// AgonesSDK Emulator Return GameServer Namespace
    /// </summary>
    public string EmulateSdkNameSpace { get; set; } = "DummyGameServer";
    /// <summary>
    /// AgonesSDK Emulator Return GameServer port
    /// </summary>
    public int EmulateSdkPort { get; set; } = 80;
    /// <summary>
    /// true to emulate shutdown = never shutdown. false to shutdown via Environment.Exit
    /// </summary>
    public bool EmulateSdkNoShutdown { get; set; }
    /// <summary>
    /// Healthcheck Initial Delay period to begin.
    /// </summary>
    public TimeSpan HealthcheckDelay { get; set; } = TimeSpan.FromSeconds(5);
    /// <summary>
    /// Healthcheck interval period.
    /// </summary>
    public TimeSpan HealthcheckInterval { get; set; } = TimeSpan.FromSeconds(5);
}
