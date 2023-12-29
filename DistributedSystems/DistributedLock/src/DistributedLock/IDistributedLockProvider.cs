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
    /// <param name="cancellationToken">Specifies a token by which the wait can be canceled</param>
    /// <returns>An <see cref="IDistributedLockHandle"/> which can be used to release the lock</returns>
    /// <exception cref="DistributedResourceException">When unable to acquire the distributed lock.</exception>
    Task<IDistributedLockHandle> AcquireAsync(string name, CancellationToken cancellationToken = default);
}