namespace SimpleServer.AgonesAspNetCore;

public class AgonesOption
{
    /// <summary>
    /// Healthcheck Initial Delay period to begin.
    /// </summary>
    public TimeSpan HealthcheckDelay { get; set; } = TimeSpan.FromSeconds(5);
    /// <summary>
    /// Healthcheck interval period.
    /// </summary>
    public TimeSpan HealthcheckInterval { get; set; } = TimeSpan.FromSeconds(5);
}
