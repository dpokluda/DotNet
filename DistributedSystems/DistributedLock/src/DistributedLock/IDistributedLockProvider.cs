namespace DistributedLock;

/// <summary>
/// Acts as a factory for acquiring a distributed lock.
/// </summary>
public interface IDistributedLockProvider
{
    /// <summary>
    /// Acquires the lock asynchronously, failing with <see cref="TimeoutException"/> if the attempt times out. 
    /// </summary>
    /// <param name="name">Identifier of the distributed lock to acquire.</param>
    /// <param name="cancellationToken">Specifies a token by which the wait can be canceled</param>
    /// <returns>An <see cref="IDistributedLockHandle"/> which can be used to release the lock</returns>
    Task<IDistributedLockHandle> AcquireAsync(string name, CancellationToken cancellationToken = default);
}