namespace MiniTwitch.Helix.Enums;

public enum ConduitShardStatus
{
    Enabled,
    WebhookCallbackVerificationPending,
    WebhookCallbackVerificationFailed,
    NotificationFailuresExceeded,
    WebsocketDisconnected,
    WebsocketFailedPingPong,
    WebsocketReceivedInboundTraffic,
    WebsocketInternalError,
    WebsocketNetworkTimeout,
    WebsocketNetworkError,
    WebsocketFailedToReconnect,
    WebsocketConnectionUnused
}
