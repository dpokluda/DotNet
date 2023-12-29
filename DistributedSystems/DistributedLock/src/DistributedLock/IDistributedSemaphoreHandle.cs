namespace DistributedLock;

/// <summary>
/// A handle to a distributed semaphore. To unlock/release, simply dispose the handle.
/// </summary>
public interface IDistributedSemaphoreHandle : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Release distributed semaphore asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Specifies a token by which the wait can be canceled</param>
    Task ReleaseAsync(CancellationToken cancellationToken = default);
}