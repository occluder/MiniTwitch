using System.Text.Json.Serialization;
using MiniTwitch.Helix.Enums;
using MiniTwitch.Helix.Internal.Json;

namespace MiniTwitch.Helix.Requests;

public class SuspiciousUserStatusUpdate(long userId, SuspiciousUserStatus status)
{
    [JsonConverter(typeof(LongConverter))]
    public long UserId { get; } = userId;
    public string Status { get; } = status.ToString();
}
