using DistributedLock.Exceptions;

namespace DistributedLock;

/// <summary>
/// Acts as a factory for acquiring a distributed semaphore.
/// </summary>
public interface IDistributedSemaphoreProvider
{
    /// <summary>
    /// Acquires the semaphore asynchronously, failing with <see cref="DistributedResourceException"/> if the attempt fails. 
    /// </summary>
    /// <param name="name">Identifier of the distributed semaphore to acquire.</param>
    /// <param name="maxCount">Maximum count of parallel semaphore acquisitions.</param>
    /// <param name="cancellationToken">Specifies a token by which the wait can be canceled</param>
    /// <returns>An <see cref="IDistributedSemaphoreHandle"/> which can be used to release the semaphore</returns>
    /// <exception cref="DistributedResourceException">When unable to acquire the distributed semaphore.</exception>
    Task<IDistributedSemaphoreHandle> AcquireAsync(string name, int maxCount, CancellationToken cancellationToken = default);
}