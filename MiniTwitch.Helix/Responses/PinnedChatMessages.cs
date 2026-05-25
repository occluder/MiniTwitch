using System.Text.Json.Serialization;
using MiniTwitch.Helix.Enums;
using MiniTwitch.Helix.Internal.Json;
using MiniTwitch.Helix.Models;

namespace MiniTwitch.Helix.Responses;

public class PinnedChatMessages : BaseResponse<PinnedChatMessages.Pin>
{
    public record Pin(
        [property: JsonPropertyName("message_id")]
        string MessageId,
        [property: JsonPropertyName("broadcaster_id")]
        long BroadcasterId,
        [property: JsonPropertyName("sender_user_id")]
        long SenderUserId,
        [property: JsonPropertyName("sender_user_login")]
        string SenderUsername,
        [property: JsonPropertyName("sender_user_name")]
        string SenderUserDisplayName,
        [property: JsonPropertyName("pinned_by_user_id")]
        long PinnedByUserId,
        [property: JsonPropertyName("pinned_by_user_login")]
        string PinnedByUsername,
        [property: JsonPropertyName("pinned_by_user_name")]
        string PinnedByUserDisplayName,
        [property: JsonPropertyName("message")]
        Message Message,
        [property: JsonPropertyName("starts_at")]
        DateTimeOffset StartsAt,
        [property: JsonPropertyName("ends_at")]
        DateTimeOffset? EndsAt,
        [property: JsonPropertyName("updated_at")]
        DateTimeOffset UpdatedAt
    );

    public record Message(
        [property: JsonPropertyName("text")]
        string Text,
        [property: JsonPropertyName("fragments")]
        IReadOnlyList<MessageFragment> Fragments
    );

    public record MessageFragment(
        [property: JsonPropertyName("type")]
        [property: JsonConverter(typeof(EnumConverter<MessageFragmentType>))]
        MessageFragmentType Type,
        [property: JsonPropertyName("text")]
        string Text,
        [property: JsonPropertyName("cheermote")]
        CheermoteFragment? Cheermote,
        [property: JsonPropertyName("emote")]
        EmoteFragment? Emote,
        [property: JsonPropertyName("mention")]
        MentionFragment? Mention
    );

    public record CheermoteFragment(
        [property: JsonPropertyName("prefix")]
        string Prefix,
        [property: JsonPropertyName("bits")]
        int Bits,
        [property: JsonPropertyName("tier")]
        int Tier
    );

    public record EmoteFragment(
        [property: JsonPropertyName("id")]
        string Id,
        [property: JsonPropertyName("emote_set_id")]
        string EmoteSetId,
        [property: JsonPropertyName("owner_id")]
        long OwnerId,
        [property: JsonPropertyName("format")]
        IReadOnlyList<string> Format
    );

    public record MentionFragment(
        [property: JsonPropertyName("user_id")]
        long UserId,
        [property: JsonPropertyName("user_login")]
        string Username,
        [property: JsonPropertyName("user_name")]
        string UserDisplayName
    );
}