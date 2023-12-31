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

    /// <summary>
    /// Checks asynchronously whether the current lock is still acquired or not.
    /// </summary>
    /// <param name="cancellationToken">Specifies a token by which the wait can be canceled</param>
    /// <returns>Boolean value <c>true</c> if it is still acquired; otherwise <c>false</c>.</returns>
    Task<bool> IsStillValidAsync(CancellationToken cancellationToken = default);
}