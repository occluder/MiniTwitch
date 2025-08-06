namespace MiniTwitch.Common;

public interface IByteMemoryConsumer
{
    public void Consume(ReadOnlyMemory<byte> data);
}
