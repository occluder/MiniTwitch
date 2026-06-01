# MiniTwitch.Irc

MiniTwitch.Irc is the component responsible for Twitch chat services. The usage of this package revolves around the `IrcClient` class which handles connection, communication, and channel management.

## Features

- Fully documented with XML comments
- Complete coverage of Twitch chat messages and events with convenient APIs
- High-performance parsing designed for speed and low memory allocation
- Asynchronous event model using `Func<T..., ValueTask>` delegates for efficient concurrency
- Automatic reconnection upon disconnection and automatic channel rejoining
- Configurable rate-limiting for sending messages and joining channels
- Anonymous connection support: no authorization needed for read-only usage
- Pluggable logging via `ILogger`: works with any logging framework
- Benchmarks available [here](https://github.com/occluder/MiniTwitch/blob/master/MiniTwitch.Irc.Benchmarks/README.md)

## Getting Started

Here is an example usage of the `IrcClient` class:

```c#
using MiniTwitch.Irc;
using MiniTwitch.Irc.Models;

namespace MiniTwitchExample;

public class Program
{
    static async Task Main()
    {
        IrcClient client = new(options =>
        {
            options.Username = "myusername";
            options.OAuth = "myoauth";
        });

        client.OnMessage += OnMessage;

        await client.ConnectAsync();
        await client.JoinChannel("yourchannel");

        await Task.Delay(-1);
    }

    private static async ValueTask OnMessage(Privmsg message)
    {
        if (message.Content.Equals("!hello", StringComparison.OrdinalIgnoreCase))
        {
            await message.ReplyWith($"Hello @{message.Author.Name}!");
        }
    }
}
```

## Message Interception

The `IrcClient.Interceptor` property allows you to inspect or filter IRC messages before they are parsed or dispatched as events. Derive from `IrcMessageInterceptor` and override `BeforeParse` and/or `AfterParse`:

```c#
using MiniTwitch.Irc;
using MiniTwitch.Irc.Models;

public class LoggingInterceptor : IrcMessageInterceptor
{
    public override bool BeforeParse(IrcClient source, ReadOnlySpan<byte> message)
    {
        Console.WriteLine($"RAW: {System.Text.Encoding.UTF8.GetString(message)}");
        return true; // false to skip parsing this message entirely
    }

    public override bool AfterParse<T>(IrcClient source, T result, ReadOnlySpan<byte> message)
    {
        if (result is Privmsg privmsg && privmsg.Author.Name == "someuser")
        {
            return false; // suppress messages from this user
        }
        return true;
    }
}

// Assign it to the client
client.Interceptor = new LoggingInterceptor();
```
