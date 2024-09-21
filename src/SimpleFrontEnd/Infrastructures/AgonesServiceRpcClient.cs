using MagicOnion.Client;
using SimpleShared;

namespace SimpleFrontEnd.Infrastructures;

public enum AgonesResponseTypes
{
    Success,
    Failed,
}
public class AgonesSdkServiceResponse
{
    public AgonesResponseTypes Status => IsSuccess ? AgonesResponseTypes.Success : AgonesResponseTypes.Failed;
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = default!;
    public string Detail { get; set; } = default!;
}

public class AgonesServiceRpcClient(ILogger<AgonesServiceRpcClient> logger)
{
    public async Task<AgonesSdkServiceResponse> AllocateCrdAsync(string address)
    {
        try
        {
            logger.LogInformation($"Sending {nameof(AllocateCrdAsync)} to {address}.");
            var channel = GrpcChannelPool.Instance.CreateChannel(address);
            var service = MagicOnionClient.Create<IAgonesService>(channel);
            var result = await service.AllocateAsync();
            return new AgonesSdkServiceResponse
            {
                IsSuccess = result.IsSuccess,
                Message = "Status change to Allocated",
                Detail = result.Detail,
            };
        }
        catch (Exception ex)
        {
            return new AgonesSdkServiceResponse
            {
                IsSuccess = false,
                Message = $"Failed to run {nameof(AllocateCrdAsync)}. {ex.Message}",
                Detail = ex.StackTrace ?? "",
            };
        }
    }

    public async Task<AgonesSdkServiceResponse> ReadyAsync(string address)
    {
        try
        {
            logger.LogInformation($"Sending {nameof(ReadyAsync)} to {address}.");
            var channel = GrpcChannelPool.Instance.CreateChannel(address);
            var service = MagicOnionClient.Create<IAgonesService>(channel);
            var result = await service.ReadyAsync();
            return new AgonesSdkServiceResponse
            {
                IsSuccess = result.IsSuccess,
                Message = $"Status change to Ready.",
                Detail = result.Detail,
            };
        }
        catch (Exception ex)
        {
            return new AgonesSdkServiceResponse
            {
                IsSuccess = false,
                Message = $"Failed to run {nameof(ReadyAsync)}. {ex.Message}",
                Detail = ex.StackTrace ?? "",
            };
        }
    }

    public async Task<AgonesSdkServiceResponse> GetGameServerAsync(string address)
    {
        try
        {
            logger.LogInformation($"Sending {nameof(GetGameServerAsync)} to {address}.");
            var channel = GrpcChannelPool.Instance.CreateChannel(address);
            var service = MagicOnionClient.Create<IAgonesService>(channel);
            var result = await service.GetGameServerAsync();
            return new AgonesSdkServiceResponse
            {
                IsSuccess = result.IsSuccess,
                Message = "Obtain list of GameServers",
                Detail = result.Detail,
            };
        }
        catch (Exception ex)
        {
            return new AgonesSdkServiceResponse
            {
                IsSuccess = false,
                Message = $"Failed to run {nameof(GetGameServerAsync)}. {ex.Message}",
                Detail = ex.StackTrace ?? "",
            };
        }
    }

    public async Task<AgonesSdkServiceResponse> ShutdownAsync(string address)
    {
        try
        {
            logger.LogInformation($"Sending {nameof(ShutdownAsync)} to {address}.");
            var channel = GrpcChannelPool.Instance.CreateChannel(address);
            var service = MagicOnionClient.Create<IAgonesService>(channel);
            var result = await service.ShutdownAsync();

            return new AgonesSdkServiceResponse
            {
                IsSuccess = result.IsSuccess,
                Message = $"Status change to Shutdown.",
                Detail = result.Detail,
            };

        }
        catch (Exception ex)
        {
            return new AgonesSdkServiceResponse
            {
                IsSuccess = false,
                Message = $"Failed to run {nameof(ShutdownAsync)}. {ex.Message}",
                Detail = ex.StackTrace ?? "",
            };
        }
    }
}
