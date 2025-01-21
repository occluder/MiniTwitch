namespace MiniTwitch.Helix.Requests;

public class NewSubscription
{
    public string Type { get; }
    public string Version { get; }
    public EventsubTransport Transport { get; }
    public object Condition { get; }

    public readonly struct EventsubTransport
    {
        public string Method { get; }
        public string? Callback { get; }
        public string? Secret { get; }
        public string? SessionId { get; }
        public string? ConduitId { get; }

        public EventsubTransport(
            string method,
            string? callback = null,
            string? secret = null,
            string? sessionId = null,
            string? conduitId = null
        )
        {
            this.Method = method;
            this.Callback = callback;
            this.Secret = secret;
            this.SessionId = sessionId;
            this.ConduitId = conduitId;
        }
    }

    public NewSubscription(string type, string version, EventsubTransport transport, object condition)
    {
        this.Type = type;
        this.Version = version;
        this.Transport = transport;
        this.Condition = condition;
    }
}
