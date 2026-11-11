namespace MiniTwitch.Helix.Enums;

public enum EventSubStatus
{
    Enabled,
    WebhookCallbackVerificationPending,
    WebhookCallbackVerificationFailed,
    NotificationFailuresExceeded,
    AuthorizationRevoked,
    ModeratorRemoved,
    GuestStarModeratorRemoved,
    UserRemoved,
    ChatUserBanned,
    VersionRemoved,
    BetaMaintenance,
    WebsocketDisconnected,
    WebsocketFailedPingPong,
    WebsocketReceivedInboundTraffic,
    WebsocketConnectionUnused,
    WebsocketInternalError,
    WebsocketNetworkTimeout,
    WebsocketNetworkError,
    WebsocketFailedToReconnect,
    ConduitDeleted
}
