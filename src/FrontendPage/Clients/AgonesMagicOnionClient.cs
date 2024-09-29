using FrontendPage.Infrastructures;
using MagicOnion.Client;
using Shared;

namespace FrontendPage.Clients;

public enum AgonesResponseTypes
{
    Success,
    Failed,
}
public record AgonesSdkServiceResponse
{
    public AgonesResponseTypes Status => IsSuccess ? AgonesResponseTypes.Success : AgonesResponseTypes.Failed;
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = default!;
    public string Detail { get; set; } = default!;
}

public class AgonesMagicOnionClient()
{
    public async Task<AgonesSdkServiceResponse> AllocateAsync(string address)
    {
        try
        {
            var client = CreateClient(address);
            var result = await client.AllocateAsync();
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
                Message = $"Allocate request failed. {ex.Message}",
                Detail = ex.StackTrace ?? "",
            };
        }
    }

    public async Task<AgonesSdkServiceResponse> ReadyAsync(string address)
    {
        try
        {
            var client = CreateClient(address);
            var result = await client.ReadyAsync();
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
                Message = $"Ready request failed. {ex.Message}",
                Detail = ex.StackTrace ?? "",
            };
        }
    }

    public async Task<AgonesSdkServiceResponse> GetGameServerAsync(string address)
    {
        try
        {
            var client = CreateClient(address);
            var result = await client.GetGameServerAsync();
            return new AgonesSdkServiceResponse
            {
                IsSuccess = result.IsSuccess,
                Message = "GetGameServer detail",
                Detail = result.Detail,
            };
        }
        catch (Exception ex)
        {
            return new AgonesSdkServiceResponse
            {
                IsSuccess = false,
                Message = $"GetGameServer request failed. {ex.Message}",
                Detail = ex.StackTrace ?? "",
            };
        }
    }

    public async Task<AgonesSdkServiceResponse> ShutdownAsync(string address)
    {
        try
        {
            var client = CreateClient(address);
            var result = await client.ShutdownAsync();

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
                Message = $"Shutdown request failed. {ex.Message}",
                Detail = ex.StackTrace ?? "",
            };
        }
    }

    private static IAgonesService CreateClient(string address)
    {
        var channel = GrpcChannelPool.Instance.CreateChannel(address);
        var service = MagicOnionClient.Create<IAgonesService>(channel);
        return service;
    }
}
