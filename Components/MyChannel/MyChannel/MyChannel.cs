using System.Collections.Concurrent;

namespace MyChannel;

/// <summary>
/// A simplified Channel<T> implementation (based on video https://learn.microsoft.com/en-us/shows/on-net/working-with-channels-in-net by Stephen Toub)
/// </summary>
/// <typeparam name="T">Type of the items in the channel.</typeparam>
public class MyChannel<T>
{
    private readonly ConcurrentQueue<T> _queue = new();

    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0);

    public void Write(T item)
    {
        _queue.Enqueue(item);
        _semaphore.Release();
    }

    public async Task<T> ReadAsync()
    {
        await _semaphore.WaitAsync();
        _queue.TryDequeue(out var item);
        return item;
    }
}