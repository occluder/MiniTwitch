﻿namespace MiniTwitch.Common;

public class AsyncEventCoordinator<T> : IDisposable
    where T : notnull
{
    private readonly Dictionary<int, SemaphoreSlim> _locks;

    public AsyncEventCoordinator(int initialCount = 0)
    {
        if (!typeof(T).IsEnum)
            throw new NotSupportedException($"The type {typeof(T)} is not an enum");

        T[] values = (T[])Enum.GetValues(typeof(T));
        _locks = new(values.Length);
        for (int i = 0; i < values.Length; i++)
        {
            _locks[(int)(object)values[i]] = new SemaphoreSlim(initialCount);
        }
    }

    public Task<bool> WaitFor(T value, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        int key = (int)(object)value;
        return _locks[key].WaitAsync(timeout, cancellationToken);
    }

    public bool IsClosed(T value)
    {
        int key = (int)(object)value;
        return _locks[key].CurrentCount == 0;
    }

    public void Open(T value)
    {
        int key = (int)(object)value;
        _ = _locks[key].Release();
    }

    public void OpenIfClosed(T value)
    {
        int key = (int)(object)value;
        if (_locks[key].CurrentCount == 0)
            _ = _locks[key].Release();
    }

    public void Dispose()
    {
        foreach (SemaphoreSlim s in _locks.Values)
        {
            s.Dispose();
        }

        _locks.Clear();
        GC.SuppressFinalize(this);
    }
}