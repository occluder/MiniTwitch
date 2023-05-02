﻿using MiniTwitch.Irc.Models;

namespace MiniTwitch.Irc.Internal;

internal sealed class RateLimitManager
{
    public int MessagePeriod { get; init; } = 30000;
    public int JoinPeriod { get; init; } = 10000;

    private readonly Dictionary<string, Queue<long>> _messages = new();
    private readonly Queue<long> _joins = new();
    private readonly int _joinLimit;
    private readonly int _normalLimit;
    private readonly int _modLimit;
    private readonly bool _isGlobal;

    public RateLimitManager(ClientOptions options)
    {
        _joinLimit = options.JoinRateLimit;
        _normalLimit = options.MessageRateLimit;
        _modLimit = options.ModMessageRateLimit;
        _isGlobal = options.UseGlobalRateLimit;
    }

    public bool CanSend(string channel, bool mod)
    {
        long time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if (!_messages.ContainsKey(channel))
        {
            _messages.Add(channel, new());
            if (_isGlobal)
            {
                int gMessages = CalcGlobalMessages(time);
                if (gMessages >= _modLimit)
                    return false;
                else if (gMessages >= _normalLimit && !mod)
                    return false;
            }

            RegisterSend(channel);
            return true;
        }

        int sent = _messages[channel].Count(x => time - x < this.MessagePeriod);
        int old = _messages[channel].Count - sent;

        for (int i = 0; i < old; i++)
        {
            _ = _messages[channel].Dequeue();
        }

        if (_isGlobal)
        {
            int gMessages = CalcGlobalMessages(time);
            if (gMessages >= _modLimit)
                return false;
            else if (gMessages >= _normalLimit && !mod)
                return false;

            RegisterSend(channel);
            return true;
        }

        if (mod)
        {

            if (sent < _modLimit)
            {
                RegisterSend(channel);
                return true;
            }

            return false;
        }

        if (sent < _normalLimit)
        {
            RegisterSend(channel);
            return true;
        }

        return false;
    }

    public bool CanJoin()
    {
        if (_joins.Count == 0)
        {
            RegisterJoin();
            return true;
        }

        long time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        int attempts = _joins.Count(x => time - x < this.JoinPeriod);
        int old = _joins.Count - attempts;

        for (int i = 0; i < old; i++)
        {
            _ = _joins.Dequeue();
        }

        if (attempts < _joinLimit)
        {
            RegisterJoin();
            return true;
        }

        return false;
    }

    private void RegisterSend(string channel) => _messages[channel].Enqueue(DateTimeOffset.Now.ToUnixTimeMilliseconds());

    private void RegisterJoin() => _joins.Enqueue(DateTimeOffset.Now.ToUnixTimeMilliseconds());

    private int CalcGlobalMessages(long time) => _messages.SelectMany(x => x.Value).Count(x => time - x < this.MessagePeriod);
}
