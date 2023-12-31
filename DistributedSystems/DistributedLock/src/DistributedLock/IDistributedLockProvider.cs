using DistributedLock.Exceptions;

namespace DistributedLock;

/// <summary>
/// Acts as a factory for acquiring a distributed lock.
/// </summary>
public interface IDistributedLockProvider
{
    /// <summary>
    /// Acquires the lock asynchronously, failing with <see cref="DistributedResourceException"/> if the attempt failes. 
    /// </summary>
    /// <param name="name">Identifier of the distributed lock to acquire.</param>
    /// <param name="expiration">Time after which the lock will be automatically released.</param>
    /// <param name="cancellationToken">Specifies a token by which the wait can be canceled</param>
    /// <returns>An <see cref="IDistributedLockHandle"/> which can be used to release the lock</returns>
    /// <exception cref="DistributedResourceException">When unable to acquire the distributed lock.</exception>
    Task<IDistributedLockHandle> AcquireAsync(string name, TimeSpan expiration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks asynchronously whether the lock is already acquired or not.
    /// </summary>
    /// <param name="name">Identifier of the distributed lock to acquire.</param>
    /// <param name="cancellationToken">Specifies a token by which the wait can be canceled</param>
    /// <returns>Boolean value <c>true</c> if already acquired; otherwise <c>false</c>.</returns>
    Task<bool> IsAcquiredAsync(string name, CancellationToken cancellationToken = default);
}