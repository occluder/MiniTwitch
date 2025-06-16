# About

MiniTwitch is a collection of Twitch libraries for NET Core 6.0+ with the goal of providing convenient asynchronous APIs for Twitch services and care for performance and memory.

Only the IRC and PubSub components are curently implemented, other components for remaining Twitch services are still in development (Helix, EventSub). 🛠

****

# MiniTwitch.Irc

MiniTwitch.Irc is the component responsible for Twitch chat services. The usage of this package revolves around the `IrcClient`  class which handles connection, communication and channel management

## Features

* Package code is fully documented with XML comments
* Full coverage of chatroom messages and events with convenient APIs
* Built with performance and memory in mind. [Nanosecond speeds](https://github.com/Foretack/MiniTwitch/blob/master/MiniTwitch.Irc.Benchmarks/README.md), with low memory allocation
* Exposes events as `ValueTask`, making for efficient & concurrent usage
* Automatically reconnects upon disconnection & automatically rejoins channels
* Simple & customizable ratelimiting of sending messages and joining channels
* Allows for connecting anonymously - No need for authorization if you don't plan to send anything!
* Understand what happens behind the scenes by supplying an `ILogger`. Allows you to use any logging library which implements [logging abstractions](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging)

## Getting Started

here is an example usage of the `IrcClient` class:

```c#
using MiniTwitch.Irc;
using MiniTwitch.Irc.Models;

namespace MiniTwitchExample;

public class Program
{
    static async Task Main()
    {
        Bot bot = new("myusername", "mytoken");
        await bot.Client.ConnectAsync();
        await bot.Client.JoinChannel("occluder");

        _ = Console.ReadLine();
    }
}

public class Bot
{
    public IrcClient Client { get; init; }

    public Bot(string username, string token)
    {
        Client = new IrcClient(options =>
        {
            options.Username = username;
            options.OAuth = token;
        });

        Client.OnChannelJoin += ChannelJoinEvent;
        Client.OnMessage += MessageEvent;
    }

    private ValueTask ChannelJoinEvent(IrcChannel channel)
    {
        return Client.SendMessage(channel.Name, "Hello from MiniTwitch!");
    }

    private async ValueTask MessageEvent(Privmsg message)
    {
        if (message.Content == "penis123")
        {
            await message.ReplyWith("That's my password!!");
        }
        else if (message.Content == "Wait a minute!")
        {
            await Task.Delay(TimeSpan.FromMinutes(1));
            await message.ReplyWith("I waited. Now what?");
        }
    }
}
```
****

# MiniTwitch.PubSub

MiniTwitch.PubSub is the component responsible for interaction with the Twitch PubSub service. The usage of this package revolves around the `PubSubClient`  class and the `Topics` static class.

## Features

* Package code is fully documented with XML comments
* Exposes documented & undocumented PubSub topics
* Uses `System.Text.Json` under the hood; Fast, efficient and without unnecessary dependencies
* Exposes events with `Func<T1, .., ValueTask>` delegates, making asynchronous handling very easy
* Automatically reconnects upon disconnection & automatically re-listens to topics
* Simplistic; Events have clear descriptions on what they do and how to get them invoked
* Multi-token support; You're not limited to 1 auth token per `PubSubClient`
* Comes with a built-in logger, which can be disabled or replaced easily
* Events return the topic parameters as `ChannelId` or `UserId`, making them easily distinguishable

## Getting Started

here is an example usage of the `PubSubClient` class:

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
        client.OnFollow += OnFollow;
        client.OnTimedOut += OnTimedOut;

        _ = Console.ReadLine();
    }

    private static ValueTask OnStreamUp(ChannelId channelId, IStreamUp stream)
    {
        Console.WriteLine($"Channel ID {channelId} just went live! (Stream delay: {stream.PlayDelay})");
        return ValueTask.CompletedTask;
    }

    private static ValueTask OnFollow(ChannelId channelId, Follower follower)
    {
        Console.WriteLine($"{follower.Name} just followed you!");
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

# MiniTwitch.Helix

MiniTwitch.Helix conveniently wraps the Twitch Helix API and exposes them through the `HelixWrapper`class.
## Features

- Contains all generally available and beta Helix API endpoints
- Virtually no dependencies
- Returns meaningful information about responses with `HelixResult`:
	- **HelixResult.Success**: Whether the request was successful
	- **HelixResult.StatusCode**: Status code of the response
	- **HelixResult.Message**: Contains the error message for the request, if not successful
	- **HelixResult.Elapsed**: The amount of time the request took to get a response
	- **HelixResult.RateLimit.Limit**: Maximum amount of requests that can be made in a period
	- **HelixResult.RateLimit.Remaining**: The amount of requests that can be made before the ratelimit resets
	- **HelixResult.RateLimit.ResetsIn**: The amount of time before the ratelimit resets

- Validates access tokens & warns before their expiry
- Easy pagination API for `HelixResult<T>`:
	- **HelixResult.CanPaginate**: Determines whether the next page of content can be requested
	- **HelixResult.Paginate()**: Fetches the next page of content

## Getting Started

This example demonstrates the usage of `HelixWrapper` and  pagination through `HelixResult<T>`

```csharp
using MiniTwitch.Helix;
using MiniTwitch.Helix.Models;
using MiniTwitch.Helix.Responses;

namespace MiniTwitchExample;

public class Program
{
    public static HelixWrapper Helix { get; set; }

    static async Task Main()
    {
        Helix = new HelixWrapper("mytoken", 783267696);

        var emotes = await GetAllMyEmotes();
    }

    private static async Task<List<string>> GetAllMyEmotes()
    {
        HelixResult<UserEmotes> emotesResult = await Helix.GetUserEmotes();
        if (!emotesResult.Success)
        {
            return [];
        }

        List<string> emoteList = [];
        foreach (var emote in emotesResult.Value.Data)
        {
            emoteList.Add(emote.Name);
        }

        // Fetch the next pages of content.
        // The code inside will not run if there are no more pages.
        await foreach (var nextEmotesResult in emotesResult.EnumeratePages())
        {
            foreach (var emote in nextEmotesResult.Value.Data)
            {
                emoteList.Add(emote.Name);
            }
        }

        return emoteList;
    }
}
```

****

### Subscribing to events

All events use either the `Func<T, ValueTask>`, `Func<T1, T2, ValueTask>` or `Func<ValueTask>` delegates. Meaning that subscribing methods must have the return type `ValueTask` and account for the `T` parameters when present (see `client.OnStreamUp` in the PubSub example above), the parameter name you set does not matter.

If the method doesn't have any asynchronous code, it is recommended to not mark it as `async` and return `ValueTask.CompletedTask`, `default` or some `ValueTask` object instead.


Subscribing can be done either by declaring methods:

```c#
Client.OnMessage += MessageEvent;

private async ValueTask MessageEvent(Privmsg message)
{
    ...
}
```

or with anonymous functions:

```c#
Client.OnMessage += async message => 
{
    ...
};
```

Note that you cannot unsubscribe from anonymous functions.

Read more about event subscriptions [here](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/events/how-to-subscribe-to-and-unsubscribe-from-events)

****

## Installation

You can add the packages to your project by searching for them with the package manager, or by using the NuGet cli tool:

```c#
Install-Package MiniTwitch.Irc
Install-Package MiniTwitch.PubSub
Install-Package MiniTwitch.Helix
```

## Dependencies

MiniTwitch.Common:
- [Microsoft.Extensions.Logging](https://www.nuget.org/packages/Microsoft.Extensions.Logging/)

MiniTwitch.Irc:
- [MiniTwitch.Common](https://www.nuget.org/packages/MiniTwitch.Common/)

MiniTwitch.PubSub:
- [MiniTwitch.Common](https://www.nuget.org/packages/MiniTwitch.Common/)

MiniTwitch.Helix:
- [MiniTwitch.Common](https://www.nuget.org/packages/MiniTwitch.Common/)