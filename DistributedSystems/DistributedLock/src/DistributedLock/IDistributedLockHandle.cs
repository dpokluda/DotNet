namespace DistributedLock;

/// <summary>
/// A handle to a distributed lock. To unlock/release, simply dispose the handle.
/// </summary>
public interface IDistributedLockHandle : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Release distributed lock asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Specifies a token by which the wait can be canceled</param>
    Task ReleaseAsync(CancellationToken cancellationToken = default);
}