# MiniTwitch.PubSub

MiniTwitch.PubSub is the component responsible for interaction with the Twitch PubSub service. The usage of this package revolves around the `PubSubClient` class and the `Topics` static class.

> **Note:** Twitch has decommissioned most official PubSub topics.

## Features

- Fully documented with XML comments
- Covers both documented and undocumented PubSub topics
- Built on `System.Text.Json`: fast, efficient, and without unnecessary dependencies
- Asynchronous event model using `Func<T..., ValueTask>` delegates
- Automatic reconnection upon disconnection and automatic topic re-listening
- Multi-token support: you're not limited to one auth token per `PubSubClient`
- Events receive typed identifiers (`ChannelId`, `UserId`) for easy routing
- Comes with a built-in default logger, which can be disabled or replaced
- Pluggable logging via `ILogger`: works with any logging framework
- Clear event descriptions explaining what triggers them and how to listen

## Getting Started

Here is an example usage of the `PubSubClient` class:

```c#
using MiniTwitch.PubSub;
using MiniTwitch.PubSub.Interfaces;
using MiniTwitch.PubSub.Models;
using MiniTwitch.PubSub.Payloads;

namespace MiniTwitchExample;

public class Program
{
    static async Task Main()
    {
        PubSubClient client = new("my token");

        await client.ConnectAsync();
        var playbackResponse = await client.ListenTo(Topics.VideoPlayback(36175310));
        if (playbackResponse.IsSuccess)
            Console.WriteLine($"Listened to {playbackResponse.TopicKey} successfully!");

        var responses =  await client.ListenTo(Topics.Following(783267696) | Topics.ChatroomsUser(754250938, "a different token"));
        foreach (var response in responses)
        {
            if (!response.IsSuccess)
                Console.WriteLine($"Failed to listen to {response.TopicKey}! Error: {response.Error}");
        }

        client.OnStreamUp += OnStreamUp;
        client.OnTimedOut += OnTimedOut;

        _ = Console.ReadLine();
    }

    private static ValueTask OnStreamUp(ChannelId channelId, IStreamUp stream)
    {
        Console.WriteLine($"Channel ID {channelId} just went live! (Stream delay: {stream.PlayDelay})");
        return ValueTask.CompletedTask;
    }

    private static ValueTask OnTimedOut(UserId userId, ITimeOutData timeout)
    {
        Console.WriteLine(
            $"Your other account (ID: {userId}) has been timed out for {timeout.ExpiresInMs}ms in channel ID {timeout.ChannelId}");
        return ValueTask.CompletedTask;
    }
}
```
