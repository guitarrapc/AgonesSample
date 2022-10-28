using MagicOnion;
using MagicOnion.Server;
using SimpleShared;

namespace SimpleBackendGrpc.Services;
public class EchoService : ServiceBase<IEchoService>, IEchoService
{
    public UnaryResult<string> EchoAsync(string message)
    {
        return new UnaryResult<string>($"{Environment.MachineName}@{DateTime.Now}:{message}");
    }
}
