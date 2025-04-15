using System.Text.Json.Serialization;
using MiniTwitch.Helix.Internal.Json;

namespace MiniTwitch.Helix.Requests;

public class ChatMessage(long broadcasterId, string message, string? replyParentMessageId = null, bool forSourceOnly = false)
{
    [JsonConverter(typeof(LongConverter))]
    public long BroadcasterId { get; } = broadcasterId;
    public string Message { get; } = message;
    public string? ReplyParentMessageId { get; } = replyParentMessageId;
    public bool ForSourceOnly { get; } = forSourceOnly;
    /// <summary>
    /// This value is assigned automatically
    /// </summary>
    public long SenderId { get; internal set; }
}