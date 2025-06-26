namespace MiniTwitch.PubSub.Internal.Enums;

internal enum MessageTopic
{
    None = 31200,
    Polls = 62048,
    Following = 99654,
    AutomodQueue = 131435,
    LowTrustUsers = 169344,
    ChatroomsUser = 166320,
    VideoPlayback = 228094,
    PinnedChatUpdates = 236768,
    BitsEventsV1 = 210474,
    BitsEventsV2 = 210573,
    LeaderboardEvents = 222372,
    ChannelPredictions = 243488,
    ModeratorActions = 230868,
    ChannelPoints = 240471,
    BroadcastSettingsUpdate = 250978,
    BitsBadgeUnlock = 253836,
    SubscribeEvents = 262746,
    CommunityChannelPoints = 267003,
    CommunityMoments = 277101,
    ModerationNotifications = 352521,
}
