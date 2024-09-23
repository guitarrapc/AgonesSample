using MagicOnion;

namespace Shared;

public interface IEchoService : IService<IEchoService>
{
    UnaryResult<string> EchoAsync(string message);
}
