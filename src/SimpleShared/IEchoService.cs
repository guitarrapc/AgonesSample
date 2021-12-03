using MagicOnion;

namespace SimpleShared;

public interface IEchoService : IService<IEchoService>
{
    UnaryResult<string> EchoAsync(string message);
}
