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
    /// <param name="id">Identifier of the instance (to prevent multi incrementation).</param>
    /// <param name="expiration">Time after which the semaphore will be automatically released.</param>
    /// <param name="maxValue">Maximum count of parallel semaphore acquisitions.</param>
    /// <param name="cancellationToken">Specifies a token by which the wait can be canceled</param>
    /// <returns>An <see cref="IDistributedSemaphoreHandle"/> which can be used to release the semaphore</returns>
    /// <exception cref="DistributedResourceException">When unable to acquire the distributed semaphore.</exception>
    Task<IDistributedSemaphoreHandle> AcquireAsync(string name, string id, TimeSpan expiration, int maxValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves current semaphore counter (number of successful acquisitions) asynchronously.
    /// </summary>
    /// <param name="name">Identifier of the distributed semaphore to acquire.</param>
    /// <param name="cancellationToken">Specifies a token by which the wait can be canceled</param>
    /// <returns>The distributed semaphore counter value.</returns>
    Task<int> GetCountAsync(string name, CancellationToken cancellationToken = default);}