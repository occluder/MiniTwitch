using System.Collections;
using MiniTwitch.Helix.Enums;
using MiniTwitch.Helix.Internal.Json;
using MiniTwitch.Helix.Responses;

namespace MiniTwitch.Helix.Test.DataSources;

public class PinnedMessageDataSource : IEnumerable<object[]>
{
    private static readonly PinnedChatMessages.Pin Base = new(
        MessageId: "a42a84b2-7ad7-4ac1-95bb-0843d70e005a",
        BroadcasterId: 46673989,
        SenderUserId: 77481472,
        SenderUsername: "jpsauce",
        SenderUserDisplayName: "JPSauce",
        PinnedByUserId: 82674227,
        PinnedByUsername: "jammehge",
        PinnedByUserDisplayName: "JAMMEHGE",
        Message: new PinnedChatMessages.Message(
            Text: "fix it! now!!!",
            Fragments: [
                new PinnedChatMessages.MessageFragment(
                    Type: MessageFragmentType.Text,
                    Text: "fix it! now!!!",
                    Cheermote: null,
                    Emote: null,
                    Mention: null
                )
            ]
        ),
        StartsAt: new DateTimeOffset(2026, 05, 16, 23, 10, 16, TimeSpan.Zero),
        EndsAt: new DateTimeOffset(2026, 01, 02, 03, 04, 55, TimeSpan.Zero),
        UpdatedAt: new DateTimeOffset(2026, 01, 02, 03, 04, 55, TimeSpan.Zero)
    );

    private static string JsonFromModel(PinnedChatMessages.Pin pin) =>
        $$"""
          {
            "data": [
              {
                "broadcaster_id": "{{pin.BroadcasterId}}",
                "ends_at": {{(pin.EndsAt is {} ea ? $"\"{ea:yyyy-MM-ddTHH:mm:sszzz}\"" : "null")}},
                "message": {
                  "fragments": [
                      {{string.Join(", ", pin.Message.Fragments.Select(BuildFragment))}}
                  ],
                  "text": "{{pin.Message.Text}}"
                },
                "message_id": "{{pin.MessageId}}",
                "pinned_by_user_id": "{{pin.PinnedByUserId}}",
                "pinned_by_user_login": "{{pin.PinnedByUsername}}",
                "pinned_by_user_name": "{{pin.PinnedByUserDisplayName}}",
                "sender_user_id": "{{pin.SenderUserId}}",
                "sender_user_login": "{{pin.SenderUsername}}",
                "sender_user_name": "{{pin.SenderUserDisplayName}}",
                "starts_at": "{{pin.StartsAt:yyyy-MM-ddTHH:mm:sszzz}}",
                "updated_at": "{{pin.UpdatedAt:yyyy-MM-ddTHH:mm:sszzz}}"
              }
            ]
          }
          """;

    private static string BuildFragment(PinnedChatMessages.MessageFragment fragment) =>
        $$"""
          {
            "cheermote": {{(fragment.Cheermote is { } c ? $"{{\"prefix\": \"{c.Prefix}\", \"bits\": {c.Bits}, \"tier\": {c.Tier}}}" : "null")}},
            "emote": {{(fragment.Emote is { } e ? $"{{\"id\": \"{e.Id}\", \"emote_set_id\": \"{e.EmoteSetId}\", \"owner_id\": \"{e.OwnerId}\", \"format\": [{string.Join(", ", e.Format.Select(f => $"\"{f}\""))}]}}" : "null")}},
            "mention": {{(fragment.Mention is { } m ? $"{{\"user_id\": \"{m.UserId}\", \"user_login\": \"{m.Username}\", \"user_name\": \"{m.UserDisplayName}\"}}" : "null")}},
            "text": "{{fragment.Text}}",
            "type": "{{SnakeCase.Instance.ConvertToCase(fragment.Type.ToString())}}"
          }
          """;

    private static IEnumerable<PinnedChatMessages.Pin> Cases =>
    [
        Base with { EndsAt = null },
        Base with
        {
            Message = Base.Message with
            {
                Fragments =
                [
                    Base.Message.Fragments[0] with
                    {
                        Type = MessageFragmentType.Cheermote,
                        Cheermote = new PinnedChatMessages.CheermoteFragment("prefix_1", 1, 2),
                        Emote = null,
                        Mention = null
                    }
                ]
            }
        },
        Base with
        {
            Message = Base.Message with
            {
                Fragments =
                [
                    Base.Message.Fragments[0] with
                    {
                        Type = MessageFragmentType.Emote,
                        Cheermote = null,
                        Emote = new PinnedChatMessages.EmoteFragment("emoteId_1", "emoteSetId_1", 1, ["format_1"]),
                        Mention = null
                    }
                ]
            }
        },
        Base with
        {
            Message = Base.Message with
            {
                Fragments =
                [
                    Base.Message.Fragments[0] with
                    {
                        Type = MessageFragmentType.Mention,
                        Cheermote = null,
                        Emote = null,
                        Mention = new PinnedChatMessages.MentionFragment(1, "username_1", "userDisplayName_1")
                    }
                ]
            }
        }
    ];

    public IEnumerator<object[]> GetEnumerator() => Cases
        .Select(@case => new object[]
        {
            JsonFromModel(@case),
            new PinnedChatMessages { Data = [@case] }
        })
        .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}