using Agones;
using Microsoft.Extensions.Options;

namespace AgonesAspNetCore;

public class AgonesHealthCheckService(IHostApplicationLifetime applicationLifetime, IAgonesSDK agonesSdk, IOptions<AgonesOptions> options, AgonesCondition condition, ILogger<AgonesHealthCheckService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!applicationLifetime.ApplicationStarted.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
        }

        // initial delay before begin health check
        await Task.Delay(options.Value.HealthcheckDelay, stoppingToken).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);

        logger.LogInformation("Initializing AgonesSdk.");
        await InitializeAsync().ConfigureAwait(false);

        logger.LogInformation("Begin health checker.");
        await MonitorAsync(stoppingToken).ConfigureAwait(false);

        logger.LogInformation($"Stopped health checker. Cancelled: {stoppingToken.IsCancellationRequested}");
    }

    private async Task InitializeAsync()
    {
        logger.LogInformation("Connect to AgonesSdk.");

        // Let's connect to Agones Sdk.
        condition.Connected(true);
        if (!condition.IsConnected)
        {
            logger.LogInformation("AgonesSdk connection failed.");
            throw new OperationCanceledException(nameof(AgonesHealthCheckService));
        }

        // Set Status to ready.
        logger.LogInformation("AgonesSdk connection success change state to Ready.");
        await agonesSdk.ReadyAsync().ConfigureAwait(false);
    }

    private async Task MonitorAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Health checking with AgonesSdk.");
        // check both AgonesSdk's cancellation token and Keeper's cancellation token.
        using var timer = new PeriodicTimer(options.Value.HealthcheckInterval);

        // loop to keep health check
        var healthCheckFailedCount = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var status = await agonesSdk.HealthAsync().ConfigureAwait(false);
                condition.Healthy(status.StatusCode);
                // reset counter if health check success.
                if (condition.IsHealthy)
                {
                    healthCheckFailedCount = 0;
                }
            }
            catch (HttpRequestException ex)
            {
                healthCheckFailedCount++;
                logger.LogError(ex.Message, ex);
            }
            catch (Exception ex)
            {
                healthCheckFailedCount++;
                logger.LogError(ex.Message, ex);
            }
            finally
            {
                // if health check failed more than limit, change state to Unhealthy.
                if (healthCheckFailedCount >= options.Value.HealthcheckFailureThreshold)
                {
                    condition.SetUnhealthy();
                }
            }

            await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
