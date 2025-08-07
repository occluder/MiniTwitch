using Microsoft.Extensions.Logging;

namespace MiniTwitch.Common;

public interface IWebSocketConsumer
{
    public void BytesReceived(ReadOnlyMemory<byte> data);
    public Task ConnectionEstablished();
    public Task ConnectionReestablished();
    public Task Disconnection();
    public void ReadLogMessage(LogLevel level, string message, params object[] args);
    public void ReadLogException(Exception ex, string message, params object[] args);
}
