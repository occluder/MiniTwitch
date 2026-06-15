using System.Text.Json.Serialization;
using MiniTwitch.EventSub.Internal;

namespace MiniTwitch.EventSub.Models;

/// <summary>
/// A notification when EventSub disables a shard due to the status of the underlying transport changing
/// </summary>
[EventSubEvent("conduit.shard.disabled", "1")]
public partial struct ConduitShardDisabled
{
    /// <summary>The ID of the conduit</summary>
    public partial string ConduitId { get; }
    /// <summary>The ID of the disabled shard</summary>
    public partial string ShardId { get; }
    /// <summary>The new status of the transport</summary>
    [Intern]
    public partial string Status { get; }
    /// <summary>The disabled transport</summary>
    public partial DisabledTransport Transport { get; }

    /// <summary>The disabled transport</summary>
    [EventProperty]
    public partial struct DisabledTransport
    {
        /// <summary>The transport method. Possible values: <c>websocket</c>, <c>webhook</c></summary>
        [Intern]
        public partial string Method { get; }
        /// <summary>Webhook callback URL. Null if method is <c>websocket</c></summary>
        public partial string? Callback { get; }
        /// <summary>WebSocket session ID. Null if method is <c>webhook</c></summary>
        public partial string? SessionId { get; }
        /// <summary>Time that the WebSocket session connected. Null if method is <c>webhook</c></summary>
        public partial DateTimeOffset? ConnectedAt { get; }
        /// <summary>Time that the WebSocket session disconnected. Null if method is <c>webhook</c></summary>
        public partial DateTimeOffset? DisconnectedAt { get; }
    }
}
