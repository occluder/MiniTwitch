# About

MiniTwitch is a collection of Twitch libraries for .NET 8.0+ with the goal of providing convenient asynchronous APIs for Twitch services while caring about performance and memory.

Irc, PubSub, and Helix are currently implemented and available as NuGet packages.

***

# Projects

- **[MiniTwitch.Irc](MiniTwitch.Irc/README.md)**: Twitch chat (IRC) client with full message and event coverage, high-performance parsing, automatic reconnection, and configurable rate-limiting.

- **[MiniTwitch.PubSub](MiniTwitch.PubSub/README.md)**: Twitch PubSub client supporting documented and undocumented topics, multi-token authentication, and automatic topic re-listening.

- **[MiniTwitch.Helix](MiniTwitch.Helix/README.md)**: Complete wrapper around the Twitch Helix API with pagination, rate-limit tracking, and automatic token validation.

- **MiniTwitch.Common**: Shared utilities including WebSocket client, async event coordination, logging, and extension methods used by packages.

## Subscribing to events

All events use `Func<T..., ValueTask>` delegates. Subscribing methods must return `ValueTask` and match the parameter types.

If the method has no asynchronous code, return `ValueTask.CompletedTask` or `default` instead of marking it as `async`.

Subscribing with a named method:

```c#
Client.OnMessage += MessageEvent;

private async ValueTask MessageEvent(Privmsg message)
{
    // ...
}
```

Subscribing with an anonymous function:

```c#
Client.OnMessage += async message =>
{
    // ...
};
```

Note that you cannot unsubscribe from anonymous functions.

Read more about event subscriptions [here](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/events/how-to-subscribe-to-and-unsubscribe-from-events).

***

## Installation

Add the packages to your project via the NuGet package manager or CLI:

```
Install-Package MiniTwitch.Irc
Install-Package MiniTwitch.PubSub
Install-Package MiniTwitch.Helix
```

## Dependencies

**MiniTwitch.Common:**
- [Microsoft.Extensions.Logging](https://www.nuget.org/packages/Microsoft.Extensions.Logging/)

**MiniTwitch.Irc:**
- [MiniTwitch.Common](https://www.nuget.org/packages/MiniTwitch.Common/)

**MiniTwitch.PubSub:**
- [MiniTwitch.Common](https://www.nuget.org/packages/MiniTwitch.Common/)

**MiniTwitch.Helix:**
- [MiniTwitch.Common](https://www.nuget.org/packages/MiniTwitch.Common/)
