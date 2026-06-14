using MiniTwitch.EventSub.Models;

namespace MiniTwitch.EventSub.Test;

public static class Payloads
{
    [EventPayload(typeof(ChannelChatMessage))]
    public const string ChannelChatMessageBasicJson = """
    {
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "testbroadcaster",
        "broadcaster_user_name": "TestBroadcaster",
        "chatter_user_id": "456",
        "chatter_user_login": "testchatter",
        "chatter_user_name": "TestChatter",
        "message_id": "550e8400-e29b-41d4-a716-446655440000",
        "message": {
            "text": "Hello world!",
            "fragments": [
                {
                    "type": "text",
                    "text": "Hello world!"
                }
            ],
            "message_type": "text",
            "badges": [],
            "cheer": null,
            "color": "",
            "reply": null,
            "channel_points_custom_reward_id": null,
            "source_broadcaster_user_id": null,
            "source_broadcaster_user_login": null,
            "source_broadcaster_user_name": null,
            "source_message_id": null,
            "source_badges": null,
            "is_source_only": null
        }
    }
    """;

    [EventPayload(typeof(ChannelChatMessage))]
    public const string ChannelChatMessageWithCheerJson = """
    {
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "broadcaster",
        "broadcaster_user_name": "Broadcaster",
        "chatter_user_id": "789",
        "chatter_user_login": "cheeruser",
        "chatter_user_name": "CheerUser",
        "message_id": "660e8400-e29b-41d4-a716-446655440001",
        "message": {
            "text": "Cheer100 Hello everyone!",
            "fragments": [
                {
                    "type": "cheermote",
                    "text": "Cheer100",
                    "cheermote": {
                        "prefix": "Cheer",
                        "bits": 100,
                        "tier": 1
                    },
                    "emote": null
                },
                {
                    "type": "text",
                    "text": " Hello everyone!",
                    "cheermote": null,
                    "emote": null
                }
            ],
            "message_type": "text",
            "badges": [
                {
                    "set_id": "broadcaster",
                    "id": "1",
                    "info": ""
                }
            ],
            "cheer": {
                "bits": 100
            },
            "color": "#FF0000",
            "reply": null,
            "channel_points_custom_reward_id": null,
            "source_broadcaster_user_id": null,
            "source_broadcaster_user_login": null,
            "source_broadcaster_user_name": null,
            "source_message_id": null,
            "source_badges": null,
            "is_source_only": null
        }
    }
    """;

    [EventPayload(typeof(ChannelChatMessage))]
    public const string ChannelChatMessageWithReplyJson = """
    {
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "broadcaster",
        "broadcaster_user_name": "Broadcaster",
        "chatter_user_id": "101",
        "chatter_user_login": "replyuser",
        "chatter_user_name": "ReplyUser",
        "message_id": "770e8400-e29b-41d4-a716-446655440002",
        "message": {
            "text": "@user Great point!",
            "fragments": [
                {
                    "type": "mention",
                    "text": "@user",
                    "mention": {
                        "user_id": "202",
                        "user_login": "user",
                        "user_name": "User"
                    },
                    "cheermote": null,
                    "emote": null
                },
                {
                    "type": "text",
                    "text": " Great point!",
                    "cheermote": null,
                    "emote": null
                }
            ],
            "message_type": "text",
            "badges": [],
            "cheer": null,
            "color": "#0000FF",
            "reply": {
                "parent_message_id": "550e8400-e29b-41d4-a716-446655440000",
                "parent_message_body": "Hello world!",
                "parent_user_id": "456",
                "parent_user_login": "testchatter",
                "parent_user_name": "TestChatter",
                "thread_message_id": "550e8400-e29b-41d4-a716-446655440000",
                "thread_user_id": "456",
                "thread_user_login": "testchatter",
                "thread_user_name": "TestChatter"
            },
            "channel_points_custom_reward_id": null,
            "source_broadcaster_user_id": null,
            "source_broadcaster_user_login": null,
            "source_broadcaster_user_name": null,
            "source_message_id": null,
            "source_badges": null,
            "is_source_only": null
        }
    }
    """;

    [EventPayload(typeof(ChannelChatMessage))]
    public const string ChannelChatMessageMinimalJson = """
    {
        "broadcaster_user_id": "42",
        "broadcaster_user_login": "simple",
        "broadcaster_user_name": "Simple",
        "chatter_user_id": "1",
        "chatter_user_login": "user",
        "chatter_user_name": "User",
        "message_id": "880e8400-e29b-41d4-a716-446655440003",
        "message": {
            "text": "Hi",
            "fragments": [
                {
                    "type": "text",
                    "text": "Hi"
                }
            ],
            "message_type": "text",
            "badges": [],
            "cheer": null,
            "color": "",
            "reply": null,
            "channel_points_custom_reward_id": null,
            "source_broadcaster_user_id": null,
            "source_broadcaster_user_login": null,
            "source_broadcaster_user_name": null,
            "source_message_id": null,
            "source_badges": null,
            "is_source_only": null
        }
    }
    """;

    [EventPayload(typeof(EventSubscription))]
    public const string SubscriptionWebSocketJson = """
    {
        "id": "0b7f3361-672b-4d39-b307-dd5b576c9b27",
        "status": "enabled",
        "type": "channel.chat.message",
        "version": "1",
        "condition": {
            "broadcaster_user_id": "1971641",
            "user_id": "2914196"
        },
        "transport": {
            "method": "websocket",
            "session_id": "AgoQHR3s6Mb4T8GFB1l3DlPfiRIGY2VsbC1h"
        },
        "created_at": "2023-11-06T18:11:47.492253549Z",
        "cost": 12
    }
    """;

    [EventPayload(typeof(EventSubscription))]
    public const string SubscriptionWebhookJson = """
    {
        "id": "f1c2a387-161a-49f9-a165-0f21d7a4e1c4",
        "status": "enabled",
        "type": "automod.message.hold",
        "version": "2",
        "condition": {
            "broadcaster_user_id": "1337",
            "moderator_user_id": "9001"
        },
        "transport": {
            "method": "webhook",
            "callback": "https://example.com/webhooks/callback",
            "secret": "s3cRe7"
        },
        "created_at": "2023-04-11T10:11:12.123Z",
        "cost": 0
    }
    """;

    [EventPayload(typeof(AutomodMessageHold))]
    public const string AutomodMessageHoldV1Json = """
    {
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "testbroadcaster",
        "broadcaster_user_name": "TestBroadcaster",
        "user_id": "456",
        "user_login": "baduser",
        "user_name": "BadUser",
        "message_id": "550e8400-e29b-41d4-a716-446655440000",
        "message": {
            "text": "Bad message caught by automod",
            "fragments": [
                {
                    "text": "Bad message caught by automod"
                }
            ]
        },
        "category": "aggressive",
        "level": 3,
        "held_at": "2023-04-11T10:11:12.123Z"
    }
    """;

    [EventPayload(typeof(AutomodMessageHold))]
    public const string AutomodMessageHoldV1WithCheermoteJson = """
    {
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "broadcaster",
        "broadcaster_user_name": "Broadcaster",
        "user_id": "789",
        "user_login": "cheeruser",
        "user_name": "CheerUser",
        "message_id": "660e8400-e29b-41d4-a716-446655440001",
        "message": {
            "text": "Bad message Cheer100",
            "fragments": [
                {
                    "text": "Bad message ",
                    "emote": null,
                    "cheermote": null
                },
                {
                    "text": "Cheer100",
                    "emote": null,
                    "cheermote": {
                        "prefix": "Cheer",
                        "bits": 100,
                        "tier": 1
                    }
                }
            ]
        },
        "category": "bullying",
        "level": 2,
        "held_at": "2023-04-11T10:11:12.123Z"
    }
    """;

    [EventPayload(typeof(AutomodMessageHoldV2))]
    public const string AutomodMessageHoldV2AutomodJson = """
    {
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "testbroadcaster",
        "broadcaster_user_name": "TestBroadcaster",
        "user_id": "456",
        "user_login": "baduser",
        "user_name": "BadUser",
        "message_id": "bad-message-id-1",
        "message": {
            "text": "Bad message with pogchamp",
            "fragments": [
                {
                    "type": "text",
                    "text": "Bad message with ",
                    "cheermote": null,
                    "emote": null,
                    "mention": null
                },
                {
                    "type": "cheermote",
                    "text": "pogchamp",
                    "cheermote": {
                        "prefix": "pogchamp",
                        "bits": 1000,
                        "tier": 1
                    },
                    "emote": null,
                    "mention": null
                }
            ]
        },
        "reason": "automod",
        "automod": {
            "category": "aggressive",
            "level": 1,
            "boundaries": [
                {"start_pos": 0, "end_pos": 10},
                {"start_pos": 20, "end_pos": 30}
            ]
        },
        "blocked_term": null,
        "held_at": "2023-04-11T10:11:12.123Z"
    }
    """;

    [EventPayload(typeof(AutomodMessageHoldV2))]
    public const string AutomodMessageHoldV2BlockedTermJson = """
    {
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "broadcaster",
        "broadcaster_user_name": "Broadcaster",
        "user_id": "789",
        "user_login": "baduser2",
        "user_name": "BadUser2",
        "message_id": "held-message-123",
        "message": {
            "text": "Message with blocked term",
            "fragments": [
                {
                    "type": "text",
                    "text": "Message with blocked term",
                    "cheermote": null,
                    "emote": null,
                    "mention": null
                }
            ]
        },
        "reason": "blocked_term",
        "automod": null,
        "blocked_term": {
            "terms_found": [
                {
                    "term_id": "term123",
                    "owner_broadcaster_user_id": "1337",
                    "owner_broadcaster_user_login": "broadcaster",
                    "owner_broadcaster_user_name": "Broadcaster",
                    "boundary": {
                        "start_pos": 11,
                        "end_pos": 23
                    }
                }
            ]
        },
        "held_at": "2023-04-11T10:11:12.123Z"
    }
    """;

    [EventPayload(typeof(AutomodMessageUpdate))]
    public const string AutomodMessageUpdateV1Json = """
    {
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "testbroadcaster",
        "broadcaster_user_name": "TestBroadcaster",
        "user_id": "456",
        "user_login": "baduser",
        "user_name": "BadUser",
        "moderator_user_id": "9001",
        "moderator_user_login": "the_mod",
        "moderator_user_name": "The_Mod",
        "message_id": "550e8400-e29b-41d4-a716-446655440000",
        "message": {
            "text": "This is a bad message",
            "fragments": [
                {
                    "text": "This is a bad message",
                    "emote": null,
                    "cheermote": null
                }
            ]
        },
        "level": 3,
        "category": "aggressive",
        "status": "approved",
        "held_at": "2022-12-02T15:00:00.00Z"
    }
    """;

    [EventPayload(typeof(AutomodMessageUpdate))]
    public const string AutomodMessageUpdateV1WithEmoteAndCheermoteJson = """
    {
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "broadcaster",
        "broadcaster_user_name": "Broadcaster",
        "user_id": "789",
        "user_login": "cheeruser",
        "user_name": "CheerUser",
        "moderator_user_id": "9001",
        "moderator_user_login": "the_mod",
        "moderator_user_name": "The_Mod",
        "message_id": "660e8400-e29b-41d4-a716-446655440001",
        "message": {
            "text": "Bad message Cheer100",
            "fragments": [
                {
                    "text": "Bad message ",
                    "emote": null,
                    "cheermote": null
                },
                {
                    "text": "Cheer100",
                    "emote": null,
                    "cheermote": {
                        "prefix": "Cheer",
                        "bits": 100,
                        "tier": 1
                    }
                }
            ]
        },
        "level": 2,
        "category": "bullying",
        "status": "denied",
        "held_at": "2023-04-11T10:11:12.123Z"
    }
    """;

    [EventPayload(typeof(AutomodMessageUpdateV2))]
    public const string AutomodMessageUpdateV2AutomodJson = """
    {
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "testbroadcaster",
        "broadcaster_user_name": "TestBroadcaster",
        "user_id": "4242",
        "user_login": "baduser",
        "user_name": "BadUserDisplay",
        "moderator_user_id": "9001",
        "moderator_user_login": "the_mod",
        "moderator_user_name": "The_Mod",
        "message_id": "bad-message-id-1",
        "message": {
            "text": "This is a bad message pogchamp",
            "fragments": [
                {
                    "type": "text",
                    "text": "This is a bad message ",
                    "cheermote": null,
                    "emote": null
                },
                {
                    "type": "cheermote",
                    "text": "pogchamp",
                    "cheermote": {
                        "prefix": "pogchamp",
                        "bits": 1000,
                        "tier": 1
                    },
                    "emote": null
                }
            ]
        },
        "reason": "automod",
        "status": "approved",
        "automod": {
            "category": "aggressive",
            "level": 1,
            "boundaries": [
                {"start_pos": 0, "end_pos": 10},
                {"start_pos": 20, "end_pos": 30}
            ]
        },
        "held_at": "2022-12-02T15:00:00.00Z"
    }
    """;

    [EventPayload(typeof(AutomodSettingsUpdate))]
    public const string AutomodSettingsUpdateV1Json = """
    {
        "broadcaster_user_id": "1337",
        "broadcaster_user_name": "CoolUser",
        "broadcaster_user_login": "cooluser",
        "moderator_user_id": "9001",
        "moderator_user_name": "CoolMod",
        "moderator_user_login": "coolmod",
        "overall_level": null,
        "disability": 3,
        "aggression": 3,
        "sexuality_sex_or_gender": 3,
        "misogyny": 3,
        "bullying": 3,
        "swearing": 0,
        "race_ethnicity_or_religion": 3,
        "sex_based_terms": 30
    }
    """;

    [EventPayload(typeof(AutomodSettingsUpdate))]
    public const string AutomodSettingsUpdateV1WithOverallLevelJson = """
    {
        "broadcaster_user_id": "42",
        "broadcaster_user_name": "Simple",
        "broadcaster_user_login": "simple",
        "moderator_user_id": "99",
        "moderator_user_name": "Mod",
        "moderator_user_login": "mod",
        "overall_level": 2,
        "disability": 2,
        "aggression": 2,
        "sexuality_sex_or_gender": 2,
        "misogyny": 2,
        "bullying": 2,
        "swearing": 2,
        "race_ethnicity_or_religion": 2,
        "sex_based_terms": 2
    }
    """;

    [EventPayload(typeof(AutomodMessageUpdateV2))]
    public const string AutomodMessageUpdateV2BlockedTermJson = """
    {
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "broadcaster",
        "broadcaster_user_name": "Broadcaster",
        "user_id": "789",
        "user_login": "baduser2",
        "user_name": "BadUser2",
        "moderator_user_id": "9001",
        "moderator_user_login": "the_mod",
        "moderator_user_name": "The_Mod",
        "message_id": "bad-message-id-2",
        "message": {
            "text": "Message with blocked term",
            "fragments": [
                {
                    "type": "text",
                    "text": "Message with blocked term",
                    "cheermote": null,
                    "emote": null
                }
            ]
        },
        "reason": "blocked_term",
        "status": "denied",
        "blocked_term": {
            "terms_found": [
                {
                    "term_id": "term123",
                    "owner_broadcaster_user_id": "1337",
                    "owner_broadcaster_user_login": "broadcaster",
                    "owner_broadcaster_user_name": "Broadcaster",
                    "boundary": {
                        "start_pos": 11,
                        "end_pos": 23
                    }
                }
            ]
        },
        "held_at": "2022-12-02T15:00:00.00Z"
    }
    """;

    [EventPayload(typeof(AutomodTermsUpdate))]
    public const string AutomodTermsUpdateV1AddBlockedJson = """
    {
        "broadcaster_user_id": "1337",
        "broadcaster_user_name": "TestBroadcaster",
        "broadcaster_user_login": "testbroadcaster",
        "moderator_user_id": "9001",
        "moderator_user_login": "the_mod",
        "moderator_user_name": "The_Mod",
        "action": "add_blocked",
        "from_automod": false,
        "terms": ["badword1", "badword2"]
    }
    """;

    [EventPayload(typeof(AutomodTermsUpdate))]
    public const string AutomodTermsUpdateV1RemovePermittedJson = """
    {
        "broadcaster_user_id": "42",
        "broadcaster_user_name": "Simple",
        "broadcaster_user_login": "simple",
        "moderator_user_id": "99",
        "moderator_user_login": "mod",
        "moderator_user_name": "Mod",
        "action": "remove_permitted",
        "from_automod": true,
        "terms": ["allowedword"]
    }
    """;

    [EventPayload(typeof(ChannelAdBreakBegin))]
    public const string ChannelAdBreakBeginV1ManualJson = """
    {
        "duration_seconds": 60,
        "started_at": "2019-11-16T10:11:12.634234626Z",
        "is_automatic": false,
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "cool_user",
        "broadcaster_user_name": "Cool_User",
        "requester_user_id": "1337",
        "requester_user_login": "cool_user",
        "requester_user_name": "Cool_User"
    }
    """;

    [EventPayload(typeof(ChannelBan))]
    public const string ChannelBanV1TimeoutJson = """
    {
        "user_id": "1234",
        "user_login": "cool_user",
        "user_name": "Cool_User",
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "cooler_user",
        "broadcaster_user_name": "Cooler_User",
        "moderator_user_id": "1339",
        "moderator_user_login": "mod_user",
        "moderator_user_name": "Mod_User",
        "reason": "Offensive language",
        "banned_at": "2020-07-15T18:15:11.17106713Z",
        "ends_at": "2020-07-15T18:16:11.17106713Z",
        "is_permanent": false
    }
    """;

    [EventPayload(typeof(ChannelBan))]
    public const string ChannelBanV1PermanentJson = """
    {
        "user_id": "5678",
        "user_login": "bad_actor",
        "user_name": "Bad_Actor",
        "broadcaster_user_id": "42",
        "broadcaster_user_login": "streamer",
        "broadcaster_user_name": "Streamer",
        "moderator_user_id": "99",
        "moderator_user_login": "mod",
        "moderator_user_name": "Mod",
        "reason": "Repeated harassment",
        "banned_at": "2020-08-01T12:00:00Z",
        "ends_at": null,
        "is_permanent": true
    }
    """;

    [EventPayload(typeof(ChannelAdBreakBegin))]
    public const string ChannelAdBreakBeginV1AutomaticJson = """
    {
        "duration_seconds": 30,
        "started_at": "2020-01-15T08:30:00Z",
        "is_automatic": true,
        "broadcaster_user_id": "42",
        "broadcaster_user_login": "streamer",
        "broadcaster_user_name": "Streamer",
        "requester_user_id": "42",
        "requester_user_login": "streamer",
        "requester_user_name": "Streamer"
    }
    """;

    [EventPayload(typeof(ChannelBitsUse))]
    public const string ChannelBitsUseV1CheerJson = """
    {
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "cooler_user",
        "broadcaster_user_name": "Cooler_User",
        "user_id": "1234",
        "user_login": "cool_user",
        "user_name": "Cool_User",
        "bits": 100,
        "type": "cheer",
        "message": {
            "text": "Cheer100 Hello!",
            "fragments": [
                {
                    "type": "cheermote",
                    "text": "Cheer100",
                    "cheermote": {
                        "prefix": "Cheer",
                        "bits": 100,
                        "tier": 1
                    },
                    "emote": null
                },
                {
                    "type": "text",
                    "text": " Hello!",
                    "cheermote": null,
                    "emote": null
                }
            ]
        }
    }
    """;

    [EventPayload(typeof(ChannelBitsUse))]
    public const string ChannelBitsUseV1PowerUpJson = """
    {
        "broadcaster_user_id": "42",
        "broadcaster_user_login": "streamer",
        "broadcaster_user_name": "Streamer",
        "user_id": "99",
        "user_login": "poweruser",
        "user_name": "PowerUser",
        "bits": 0,
        "type": "power_up",
        "power_up": {
            "type": "message_effect",
            "emote": null,
            "message_effect_id": "effect_123"
        }
    }
    """;

    [EventPayload(typeof(ChannelUpdate))]
    public const string ChannelUpdateV2Json = """
    {
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "cool_user",
        "broadcaster_user_name": "Cool_User",
        "title": "Best Stream Ever",
        "language": "en",
        "category_id": "12453",
        "category_name": "Grand Theft Auto",
        "content_classification_labels": ["MatureGame"]
    }
    """;

    [EventPayload(typeof(ChannelChatSettingsUpdate))]
    public const string ChannelChatSettingsUpdateV1Json = """
    {
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "cool_user",
        "broadcaster_user_name": "Cool_User",
        "emote_mode": true,
        "follower_mode": false,
        "follower_mode_duration_minutes": null,
        "slow_mode": true,
        "slow_mode_wait_time_seconds": 10,
        "subscriber_mode": false,
        "unique_chat_mode": false
    }
    """;

    [EventPayload(typeof(ChannelSubscribe))]
    public const string ChannelSubscribeV1Json = """
    {
        "user_id": "1234",
        "user_login": "cool_user",
        "user_name": "Cool_User",
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "cooler_user",
        "broadcaster_user_name": "Cooler_User",
        "tier": "1000",
        "is_gift": false
    }
    """;

    [EventPayload(typeof(ChannelSubscriptionEnd))]
    public const string ChannelSubscriptionEndV1Json = """
    {
        "user_id": "1234",
        "user_login": "cool_user",
        "user_name": "Cool_User",
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "cooler_user",
        "broadcaster_user_name": "Cooler_User",
        "tier": "1000",
        "is_gift": false
    }
    """;

    [EventPayload(typeof(ChannelSubscriptionGift))]
    public const string ChannelSubscriptionGiftV1Json = """
    {
        "user_id": "1234",
        "user_login": "cool_user",
        "user_name": "Cool_User",
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "cooler_user",
        "broadcaster_user_name": "Cooler_User",
        "total": 2,
        "tier": "1000",
        "cumulative_total": 284,
        "is_anonymous": false
    }
    """;

    [EventPayload(typeof(ChannelSubscriptionMessage))]
    public const string ChannelSubscriptionMessageV1Json = """
    {
        "user_id": "1234",
        "user_login": "cool_user",
        "user_name": "Cool_User",
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "cooler_user",
        "broadcaster_user_name": "Cooler_User",
        "tier": "1000",
        "message": {
            "text": "Love the stream!",
            "emotes": [
                {
                    "begin": 23,
                    "end": 30,
                    "id": "302976485"
                }
            ]
        },
        "cumulative_months": 15,
        "streak_months": 1,
        "duration_months": 6
    }
    """;

    [EventPayload(typeof(ChannelCheer))]
    public const string ChannelCheerV1Json = """
    {
        "is_anonymous": false,
        "user_id": "1234",
        "user_login": "cool_user",
        "user_name": "Cool_User",
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "cooler_user",
        "broadcaster_user_name": "Cooler_User",
        "message": "pogchamp",
        "bits": 1000
    }
    """;

    [EventPayload(typeof(ChannelModerate))]
    public const string ChannelModerateV1ModJson = """
    {
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "glowillig",
        "broadcaster_user_name": "glowillig",
        "moderator_user_id": "424596340",
        "moderator_user_login": "quotrok",
        "moderator_user_name": "quotrok",
        "action": "mod",
        "followers": null,
        "slow": null,
        "vip": null,
        "unvip": null,
        "mod": {
            "user_id": "141981764",
            "user_login": "twitchdev",
            "user_name": "TwitchDev"
        },
        "unmod": null,
        "ban": null,
        "unban": null,
        "timeout": null,
        "untimeout": null,
        "raid": null,
        "unraid": null,
        "delete": null,
        "automod_terms": null,
        "unban_request": null,
        "shared_chat_ban": null,
        "shared_chat_unban": null,
        "shared_chat_timeout": null,
        "shared_chat_untimeout": null,
        "shared_chat_delete": null
    }
    """;

    [EventPayload(typeof(ChannelModerate))]
    public const string ChannelModerateV1SharedChatTimeoutJson = """
    {
        "broadcaster_user_id": "423374343",
        "broadcaster_user_login": "glowillig",
        "broadcaster_user_name": "glowillig",
        "source_broadcaster_user_id": "41292030",
        "source_broadcaster_user_login": "adflynn404",
        "source_broadcaster_user_name": "adflynn404",
        "moderator_user_id": "424596340",
        "moderator_user_login": "quotrok",
        "moderator_user_name": "quotrok",
        "action": "shared_chat_timeout",
        "followers": null,
        "slow": null,
        "vip": null,
        "unvip": null,
        "mod": null,
        "unmod": null,
        "ban": null,
        "unban": null,
        "timeout": null,
        "untimeout": null,
        "raid": null,
        "unraid": null,
        "delete": null,
        "automod_terms": null,
        "unban_request": null,
        "shared_chat_ban": null,
        "shared_chat_unban": null,
        "shared_chat_timeout": {
            "user_id": "141981764",
            "user_login": "twitchdev",
            "user_name": "TwitchDev",
            "reason": "Does not like pineapple on pizza.",
            "expires_at": "2022-03-15T02:00:28Z"
        },
        "shared_chat_untimeout": null,
        "shared_chat_delete": null
    }
    """;

    [EventPayload(typeof(ChannelModerateV2))]
    public const string ChannelModerateV2WarnJson = """
    {
        "broadcaster_user_id": "423374343",
        "broadcaster_user_login": "glowillig",
        "broadcaster_user_name": "glowillig",
        "source_broadcaster_user_id": "41292030",
        "source_broadcaster_user_login": "adflynn404",
        "source_broadcaster_user_name": "adflynn404",
        "moderator_user_id": "424596340",
        "moderator_user_login": "quotrok",
        "moderator_user_name": "quotrok",
        "action": "warn",
        "followers": null,
        "slow": null,
        "vip": null,
        "unvip": null,
        "warn": {
            "user_id": "141981764",
            "user_login": "twitchdev",
            "user_name": "TwitchDev",
            "reason": "cut it out",
            "chat_rules_cited": null
        },
        "mod": null,
        "unmod": null,
        "ban": null,
        "unban": null,
        "timeout": null,
        "untimeout": null,
        "raid": null,
        "unraid": null,
        "delete": null,
        "automod_terms": null,
        "unban_request": null,
        "shared_chat_ban": null,
        "shared_chat_unban": null,
        "shared_chat_timeout": null,
        "shared_chat_untimeout": null,
        "shared_chat_delete": null
    }
    """;

    [EventPayload(typeof(ChannelCustomPowerUpRedemptionAdd))]
    public const string ChannelCustomPowerUpRedemptionAddV1Json = """
    {
        "id": "17fa2df1-ad76-4804-bfa5-a40ef63efe63",
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "cool_user",
        "broadcaster_user_name": "Cool_User",
        "user_id": "9001",
        "user_login": "cooler_user",
        "user_name": "Cooler_User",
        "user_input": "pogchamp",
        "status": "unfulfilled",
        "custom_power_up": {
            "id": "92af127c-7326-4483-a52b-b0da0be61c01",
            "title": "title",
            "bits": 100,
            "prompt": "Power-up prompt"
        },
        "redeemed_at": "2026-05-01T17:16:03.17106713Z"
    }
    """;

    [EventPayload(typeof(ChannelPointsCustomRewardRedemptionUpdate))]
    public const string ChannelPointsCustomRewardRedemptionUpdateV1Json = """
    {
        "id": "17fa2df1-ad76-4804-bfa5-a40ef63efe63",
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "cool_user",
        "broadcaster_user_name": "Cool_User",
        "user_id": "9001",
        "user_login": "cooler_user",
        "user_name": "Cooler_User",
        "user_input": "",
        "status": "fulfilled",
        "reward": {
            "id": "92af127c-7326-4483-a52b-b0da0be61c01",
            "title": "title",
            "cost": 100,
            "prompt": "reward prompt"
        },
        "redeemed_at": "2020-07-15T17:16:03.17106713Z"
    }
    """;

    [EventPayload(typeof(ChannelPointsCustomRewardRedemptionAdd))]
    public const string ChannelPointsCustomRewardRedemptionAddV1Json = """
    {
        "id": "17fa2df1-ad76-4804-bfa5-a40ef63efe63",
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "cool_user",
        "broadcaster_user_name": "Cool_User",
        "user_id": "9001",
        "user_login": "cooler_user",
        "user_name": "Cooler_User",
        "user_input": "pogchamp",
        "status": "unfulfilled",
        "reward": {
            "id": "92af127c-7326-4483-a52b-b0da0be61c01",
            "title": "title",
            "cost": 100,
            "prompt": "reward prompt"
        },
        "redeemed_at": "2020-07-15T17:16:03.17106713Z"
    }
    """;

    [EventPayload(typeof(ChannelPointsCustomRewardRemove))]
    public const string ChannelPointsCustomRewardRemoveV1Json = """
    {
        "id": "9003",
        "broadcaster_user_id": "42",
        "broadcaster_user_login": "simple",
        "broadcaster_user_name": "Simple",
        "is_enabled": true,
        "is_paused": false,
        "is_in_stock": true,
        "title": "Removed Reward",
        "cost": 50,
        "prompt": "A reward that was removed",
        "is_user_input_required": false,
        "should_redemptions_skip_request_queue": false,
        "background_color": "#FFFFFF",
        "default_image": {
            "url_1x": "https://default-1.png",
            "url_2x": "https://default-2.png",
            "url_4x": "https://default-4.png"
        }
    }
    """;

    [EventPayload(typeof(ChannelPointsCustomRewardUpdate))]
    public const string ChannelPointsCustomRewardUpdateV1Json = """
    {
        "id": "9002",
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "cool_user",
        "broadcaster_user_name": "Cool_User",
        "is_enabled": false,
        "is_paused": true,
        "is_in_stock": true,
        "title": "Updated Reward",
        "cost": 200,
        "prompt": "updated prompt",
        "is_user_input_required": false,
        "should_redemptions_skip_request_queue": true,
        "background_color": "#000000",
        "default_image": {
            "url_1x": "https://default-1.png",
            "url_2x": "https://default-2.png",
            "url_4x": "https://default-4.png"
        }
    }
    """;

    [EventPayload(typeof(ChannelPointsCustomRewardAdd))]
    public const string ChannelPointsCustomRewardAddV1Json = """
    {
        "id": "9001",
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "cool_user",
        "broadcaster_user_name": "Cool_User",
        "is_enabled": true,
        "is_paused": false,
        "is_in_stock": true,
        "title": "Cool Reward",
        "cost": 100,
        "prompt": "reward prompt",
        "is_user_input_required": true,
        "should_redemptions_skip_request_queue": false,
        "max_per_stream": {
            "is_enabled": true,
            "value": 1000
        },
        "max_per_user_per_stream": {
            "is_enabled": true,
            "value": 1000
        },
        "background_color": "#FA1ED2",
        "image": {
            "url_1x": "https://static-cdn.jtvnw.net/image-1.png",
            "url_2x": "https://static-cdn.jtvnw.net/image-2.png",
            "url_4x": "https://static-cdn.jtvnw.net/image-4.png"
        },
        "default_image": {
            "url_1x": "https://static-cdn.jtvnw.net/default-1.png",
            "url_2x": "https://static-cdn.jtvnw.net/default-2.png",
            "url_4x": "https://static-cdn.jtvnw.net/default-4.png"
        },
        "global_cooldown": {
            "is_enabled": true,
            "seconds": 1000
        },
        "cooldown_expires_at": null,
        "redemptions_redeemed_current_stream": null
    }
    """;

    [EventPayload(typeof(ChannelPointsAutomaticRewardRedemptionAddV2))]
    public const string ChannelPointsAutomaticRewardRedemptionAddV2Json = """
    {
        "broadcaster_user_id": "12826",
        "broadcaster_user_name": "Twitch",
        "broadcaster_user_login": "twitch",
        "user_id": "141981764",
        "user_name": "TwitchDev",
        "user_login": "twitchdev",
        "id": "f024099a-e0fe-4339-9a0a-a706fb59f353",
        "reward": {
            "type": "send_highlighted_message",
            "channel_points": 100,
            "emote": null
        },
        "message": {
            "text": "Hello world! VoHiYo",
            "fragments": [
                {
                    "type": "text",
                    "text": "Hello world! ",
                    "emote": null
                },
                {
                    "type": "emote",
                    "text": "VoHiYo",
                    "emote": {
                        "id": "81274"
                    }
                }
            ]
        },
        "redeemed_at": "2024-08-12T21:14:34.260398045Z"
    }
    """;

    [EventPayload(typeof(ChannelPointsAutomaticRewardRedemptionAdd))]
    public const string ChannelPointsAutomaticRewardRedemptionAddV1Json = """
    {
        "broadcaster_user_id": "12826",
        "broadcaster_user_name": "Twitch",
        "broadcaster_user_login": "twitch",
        "user_id": "141981764",
        "user_name": "TwitchDev",
        "user_login": "twitchdev",
        "id": "f024099a-e0fe-4339-9a0a-a706fb59f353",
        "reward": {
            "type": "send_highlighted_message",
            "cost": 100,
            "unlocked_emote": null
        },
        "message": {
            "text": "Hello world!",
            "emotes": [
                {
                    "id": "81274",
                    "begin": 13,
                    "end": 18
                }
            ]
        },
        "user_input": "Hello world!",
        "redeemed_at": "2024-02-23T21:14:34.260398045Z"
    }
    """;

    [EventPayload(typeof(ChannelModeratorRemove))]
    public const string ChannelModeratorRemoveV1Json = """
    {
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "cooler_user",
        "broadcaster_user_name": "Cooler_User",
        "user_id": "5678",
        "user_login": "old_mod",
        "user_name": "Old_Mod"
    }
    """;

    [EventPayload(typeof(ChannelModeratorAdd))]
    public const string ChannelModeratorAddV1Json = """
    {
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "cooler_user",
        "broadcaster_user_name": "Cooler_User",
        "user_id": "1234",
        "user_login": "mod_user",
        "user_name": "Mod_User"
    }
    """;

    [EventPayload(typeof(ChannelModerateV2))]
    public const string ChannelModerateV2ModJson = """
    {
        "broadcaster_user_id": "423374343",
        "broadcaster_user_login": "glowillig",
        "broadcaster_user_name": "glowillig",
        "moderator_user_id": "424596340",
        "moderator_user_login": "quotrok",
        "moderator_user_name": "quotrok",
        "action": "mod",
        "followers": null,
        "slow": null,
        "vip": null,
        "unvip": null,
        "warn": null,
        "mod": {
            "user_id": "141981764",
            "user_login": "twitchdev",
            "user_name": "TwitchDev"
        },
        "unmod": null,
        "ban": null,
        "unban": null,
        "timeout": null,
        "untimeout": null,
        "raid": null,
        "unraid": null,
        "delete": null,
        "automod_terms": null,
        "unban_request": null,
        "shared_chat_ban": null,
        "shared_chat_unban": null,
        "shared_chat_timeout": null,
        "shared_chat_untimeout": null,
        "shared_chat_delete": null
    }
    """;

    [EventPayload(typeof(ChannelModerate))]
    public const string ChannelModerateV1EmoteOnlyJson = """
    {
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "glowillig",
        "broadcaster_user_name": "glowillig",
        "moderator_user_id": "424596340",
        "moderator_user_login": "quotrok",
        "moderator_user_name": "quotrok",
        "action": "emoteonly",
        "followers": null,
        "slow": null,
        "vip": null,
        "unvip": null,
        "mod": null,
        "unmod": null,
        "ban": null,
        "unban": null,
        "timeout": null,
        "untimeout": null,
        "raid": null,
        "unraid": null,
        "delete": null,
        "automod_terms": null,
        "unban_request": null,
        "shared_chat_ban": null,
        "shared_chat_unban": null,
        "shared_chat_timeout": null,
        "shared_chat_untimeout": null,
        "shared_chat_delete": null
    }
    """;

    [EventPayload(typeof(ChannelUnbanRequestResolve))]
    public const string ChannelUnbanRequestResolveV1Json = """
    {
        "id": "60",
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "cool_user",
        "broadcaster_user_name": "Cool_User",
        "moderator_user_id": "1337",
        "moderator_user_login": "cool_user",
        "moderator_user_name": "Cool_User",
        "user_id": "1339",
        "user_login": "not_cool_user",
        "user_name": "Not_Cool_User",
        "resolution_text": "no",
        "status": "denied"
    }
    """;

    [EventPayload(typeof(ChannelUnbanRequestCreate))]
    public const string ChannelUnbanRequestCreateV1Json = """
    {
        "id": "60",
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "cool_user",
        "broadcaster_user_name": "Cool_User",
        "user_id": "1339",
        "user_login": "not_cool_user",
        "user_name": "Not_Cool_User",
        "text": "unban me",
        "created_at": "2023-11-16T10:11:12.634234626Z"
    }
    """;

    [EventPayload(typeof(ChannelUnban))]
    public const string ChannelUnbanV1Json = """
    {
        "user_id": "1234",
        "user_login": "cool_user",
        "user_name": "Cool_User",
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "cooler_user",
        "broadcaster_user_name": "Cooler_User",
        "moderator_user_id": "1339",
        "moderator_user_login": "mod_user",
        "moderator_user_name": "Mod_User"
    }
    """;

    [EventPayload(typeof(ChannelRaid))]
    public const string ChannelRaidV1Json = """
    {
        "from_broadcaster_user_id": "1234",
        "from_broadcaster_user_login": "cool_user",
        "from_broadcaster_user_name": "Cool_User",
        "to_broadcaster_user_id": "1337",
        "to_broadcaster_user_login": "cooler_user",
        "to_broadcaster_user_name": "Cooler_User",
        "viewers": 9001
    }
    """;

    [EventPayload(typeof(ChannelCheer))]
    public const string ChannelCheerV1AnonymousJson = """
    {
        "is_anonymous": true,
        "user_id": null,
        "user_login": null,
        "user_name": null,
        "broadcaster_user_id": "42",
        "broadcaster_user_login": "streamer",
        "broadcaster_user_name": "Streamer",
        "message": "AnonCheer",
        "bits": 500
    }
    """;

    [EventPayload(typeof(ChannelSubscriptionMessage))]
    public const string ChannelSubscriptionMessageV1NoStreakJson = """
    {
        "user_id": "5678",
        "user_login": "subscriber",
        "user_name": "Subscriber",
        "broadcaster_user_id": "42",
        "broadcaster_user_login": "streamer",
        "broadcaster_user_name": "Streamer",
        "tier": "2000",
        "message": {
            "text": "Great streams!",
            "emotes": []
        },
        "cumulative_months": 3,
        "streak_months": null,
        "duration_months": 1
    }
    """;

    [EventPayload(typeof(ChannelSubscriptionGift))]
    public const string ChannelSubscriptionGiftV1AnonymousJson = """
    {
        "user_id": null,
        "user_login": null,
        "user_name": null,
        "broadcaster_user_id": "42",
        "broadcaster_user_login": "streamer",
        "broadcaster_user_name": "Streamer",
        "total": 5,
        "tier": "3000",
        "cumulative_total": null,
        "is_anonymous": true
    }
    """;

    [EventPayload(typeof(ChannelSubscriptionEnd))]
    public const string ChannelSubscriptionEndV1GiftJson = """
    {
        "user_id": "5678",
        "user_login": "gifter",
        "user_name": "Gifter",
        "broadcaster_user_id": "42",
        "broadcaster_user_login": "streamer",
        "broadcaster_user_name": "Streamer",
        "tier": "2000",
        "is_gift": true
    }
    """;

    [EventPayload(typeof(ChannelSubscribe))]
    public const string ChannelSubscribeV1GiftJson = """
    {
        "user_id": "5678",
        "user_login": "gifter",
        "user_name": "Gifter",
        "broadcaster_user_id": "42",
        "broadcaster_user_login": "streamer",
        "broadcaster_user_name": "Streamer",
        "tier": "3000",
        "is_gift": true
    }
    """;

    [EventPayload(typeof(ChannelSharedChatEnd))]
    public const string ChannelSharedChatEndV1Json = """
    {
        "session_id": "2b64a92a-dbb8-424e-b1c3-304423ba1b6f",
        "broadcaster_user_id": "1971641",
        "broadcaster_user_login": "streamer",
        "broadcaster_user_name": "streamer",
        "host_broadcaster_user_id": "1971641",
        "host_broadcaster_user_login": "streamer",
        "host_broadcaster_user_name": "streamer"
    }
    """;

    [EventPayload(typeof(ChannelSharedChatUpdate))]
    public const string ChannelSharedChatUpdateV1Json = """
    {
        "session_id": "3c75a92a-dbb8-424e-b1c3-304423ba1b6f",
        "broadcaster_user_id": "1971641",
        "broadcaster_user_login": "streamer",
        "broadcaster_user_name": "streamer",
        "host_broadcaster_user_id": "1971641",
        "host_broadcaster_user_login": "streamer",
        "host_broadcaster_user_name": "streamer",
        "participants": [
            {
                "broadcaster_user_id": "1971641",
                "broadcaster_user_name": "streamer",
                "broadcaster_user_login": "streamer"
            },
            {
                "broadcaster_user_id": "332211",
                "broadcaster_user_name": "streamer11",
                "broadcaster_user_login": "streamer11"
            }
        ]
    }
    """;

    [EventPayload(typeof(ChannelSharedChatBegin))]
    public const string ChannelSharedChatBeginV1Json = """
    {
        "session_id": "2b64a92a-dbb8-424e-b1c3-304423ba1b6f",
        "broadcaster_user_id": "1971641",
        "broadcaster_user_login": "streamer",
        "broadcaster_user_name": "streamer",
        "host_broadcaster_user_id": "1971641",
        "host_broadcaster_user_login": "streamer",
        "host_broadcaster_user_name": "streamer",
        "participants": [
            {
                "broadcaster_user_id": "1971641",
                "broadcaster_user_name": "streamer",
                "broadcaster_user_login": "streamer"
            },
            {
                "broadcaster_user_id": "112233",
                "broadcaster_user_name": "streamer33",
                "broadcaster_user_login": "streamer33"
            }
        ]
    }
    """;

    [EventPayload(typeof(ChannelChatUserMessageUpdate))]
    public const string ChannelChatUserMessageUpdateV1Json = """
    {
        "broadcaster_user_id": "123",
        "broadcaster_user_login": "bob",
        "broadcaster_user_name": "Bob",
        "user_id": "456",
        "user_login": "tom",
        "user_name": "Tommy",
        "status": "approved",
        "message_id": "789",
        "message": {
            "text": "hey world",
            "fragments": [
                {
                    "type": "text",
                    "text": "hey world",
                    "cheermote": null,
                    "emote": null
                }
            ]
        }
    }
    """;

    [EventPayload(typeof(ChannelChatUserMessageHold))]
    public const string ChannelChatUserMessageHoldV1Json = """
    {
        "broadcaster_user_id": "123",
        "broadcaster_user_login": "bob",
        "broadcaster_user_name": "Bob",
        "user_id": "456",
        "user_login": "tom",
        "user_name": "Tommy",
        "message_id": "789",
        "message": {
            "text": "hey world",
            "fragments": [
                {
                    "type": "emote",
                    "text": "hey world",
                    "cheermote": null,
                    "emote": {
                        "id": "foo",
                        "emote_set_id": "7"
                    }
                },
                {
                    "type": "cheermote",
                    "text": "bye world",
                    "cheermote": {
                        "prefix": "prefix",
                        "bits": 100,
                        "tier": 1
                    },
                    "emote": null
                },
                {
                    "type": "text",
                    "text": "surprise",
                    "cheermote": null,
                    "emote": null
                }
            ]
        }
    }
    """;

    [EventPayload(typeof(ChannelChatSettingsUpdate))]
    public const string ChannelChatSettingsUpdateV1AllDisabledJson = """
    {
        "broadcaster_user_id": "42",
        "broadcaster_user_login": "simple",
        "broadcaster_user_name": "Simple",
        "emote_mode": false,
        "follower_mode": true,
        "follower_mode_duration_minutes": 30,
        "slow_mode": false,
        "slow_mode_wait_time_seconds": null,
        "subscriber_mode": true,
        "unique_chat_mode": true
    }
    """;

    [EventPayload(typeof(ChannelFollow))]
    public const string ChannelFollowV2Json = """
    {
        "user_id": "1234",
        "user_login": "cool_user",
        "user_name": "Cool_User",
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "cooler_user",
        "broadcaster_user_name": "Cooler_User",
        "followed_at": "2020-07-15T18:16:11.17106713Z"
    }
    """;

    [EventPayload(typeof(ChannelUpdate))]
    public const string ChannelUpdateV2NoLabelsJson = """
    {
        "broadcaster_user_id": "42",
        "broadcaster_user_login": "simple",
        "broadcaster_user_name": "Simple",
        "title": "Just Chatting",
        "language": "fr",
        "category_id": "509658",
        "category_name": "Just Chatting",
        "content_classification_labels": []
    }
    """;

    [EventPayload(typeof(ChannelChatMessageDelete))]
    public const string ChannelChatMessageDeleteV1Json = """
    {
        "broadcaster_user_id": "1337",
        "broadcaster_user_name": "Cool_User",
        "broadcaster_user_login": "cool_user",
        "target_user_id": "7734",
        "target_user_name": "Uncool_viewer",
        "target_user_login": "uncool_viewer",
        "message_id": "ab24e0b0-2260-4bac-94e4-05eedd4ecd0e"
    }
    """;

    [EventPayload(typeof(ChannelChatClearUserMessages))]
    public const string ChannelChatClearUserMessagesV1Json = """
    {
        "broadcaster_user_id": "1337",
        "broadcaster_user_name": "Cool_User",
        "broadcaster_user_login": "cool_user",
        "target_user_id": "7734",
        "target_user_name": "Uncool_viewer",
        "target_user_login": "uncool_viewer"
    }
    """;

    [EventPayload(typeof(ChannelChatClear))]
    public const string ChannelChatClearV1Json = """
    {
        "broadcaster_user_id": "1337",
        "broadcaster_user_name": "Cool_User",
        "broadcaster_user_login": "cool_user"
    }
    """;

    [EventPayload(typeof(ChannelBitsUse))]
    public const string ChannelBitsUseV1CustomPowerUpJson = """
    {
        "broadcaster_user_id": "42",
        "broadcaster_user_login": "streamer",
        "broadcaster_user_name": "Streamer",
        "user_id": "55",
        "user_login": "customuser",
        "user_name": "CustomUser",
        "bits": 500,
        "type": "custom_power_up",
        "custom_power_up": {
            "title": "Super Reward",
            "reward_id": "reward-001"
        }
    }
    """;
}
