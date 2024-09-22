using MagicOnion;
using MagicOnion.Server;
using Shared;

namespace BackendServerGrpc.Services;
public class EchoService : ServiceBase<IEchoService>, IEchoService
{
    public UnaryResult<string> EchoAsync(string message)
    {
        return new UnaryResult<string>($"{Environment.MachineName}@{DateTime.Now}:{message}");
    }
}
