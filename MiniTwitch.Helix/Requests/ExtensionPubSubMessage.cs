using System.Text.Json.Serialization;
using MiniTwitch.Helix.Internal.Json;

namespace MiniTwitch.Helix.Requests;

public readonly struct ExtensionPubSubMessage
{
    public IEnumerable<string> Target { get; }

    [JsonConverter(typeof(OptionalLongConverter))]
    public long? BroadcasterId { get; }

    public string Message { get; }

    public bool? IsGlobalBroadcast { get; }

    public ExtensionPubSubMessage(IEnumerable<string> target, long broadcasterId, string message, bool? isGlobalBroadcast = null)
    {
        this.Target = target;
        this.BroadcasterId = broadcasterId;
        this.Message = message;
        this.IsGlobalBroadcast = isGlobalBroadcast;
    }

    /// <summary>
    /// Creates a global broadcast message. The broadcaster ID is not required when <c>is_global_broadcast</c> is <see langword="true"/>.
    /// </summary>
    public ExtensionPubSubMessage(IEnumerable<string> target, string message)
    {
        this.Target = target;
        this.BroadcasterId = null;
        this.Message = message;
        this.IsGlobalBroadcast = true;
    }
}
