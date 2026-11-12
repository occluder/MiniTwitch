namespace MiniTwitch.Helix.Enums;

public enum EventSubStatus
{
    Enabled,
    WebhookCallbackVerificationPending,
    WebhookCallbackVerificationFailed,
    NotificationFailuresExceeded,
    AuthorizationRevoked,
    ModeratorRemoved,
    UserRemoved,
    VersionRemoved,
    WebsocketDisconnected,
    WebsocketFailedPingPong,
    WebsocketReceivedInboundTraffic,
    WebsocketConnectionUnused,
    WebsocketInternalError,
    WebsocketNetworkTimeout,
    WebsocketNetworkError,
    GuestStarModeratorRemoved,
    ChatUserBanned,
    BetaMaintenance,
    WebsocketFailedToReconnect,
    ConduitDeleted,
    Unknown
}
