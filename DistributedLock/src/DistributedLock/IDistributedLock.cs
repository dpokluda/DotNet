﻿namespace DistributedLock;

/// <summary>
/// A mutex synchronization primitive which can be used to coordinate access to a resource or critical region of code
/// across processes or systems. The scope and capabilities of the lock are dependent on the particular implementation
/// </summary>
public interface IDistributedLock
{
    /// <summary>
    /// A name that uniquely identifies the lock
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Attempts to acquire the lock synchronously. Usage: 
    /// <code>
    ///     using (var handle = myLock.TryAcquire(...))
    ///     {
    ///         if (handle != null) { /* we have the lock! */ }
    ///     }
    ///     // dispose releases the lock if we took it
    /// </code>
    /// </summary>
    /// <param name="timeout">How long to wait before giving up on the acquisition attempt. Defaults to 0</param>
    /// <param name="cancellationToken">Specifies a token by which the wait can be canceled</param>
    /// <returns>An <see cref="IDistributedSynchronizationHandle"/> which can be used to release the lock or null on failure</returns>
    IDistributedSynchronizationHandle? TryAcquire(TimeSpan timeout = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Acquires the lock synchronously, failing with <see cref="TimeoutException"/> if the attempt times out. Usage: 
    /// <code>
    ///     using (myLock.Acquire(...))
    ///     {
    ///         /* we have the lock! */
    ///     }
    ///     // dispose releases the lock
    /// </code>
    /// </summary>
    /// <param name="timeout">How long to wait before giving up on the acquisition attempt. Defaults to <see cref="Timeout.InfiniteTimeSpan"/></param>
    /// <param name="cancellationToken">Specifies a token by which the wait can be canceled</param>
    /// <returns>An <see cref="IDistributedSynchronizationHandle"/> which can be used to release the lock</returns>
    IDistributedSynchronizationHandle Acquire(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to acquire the lock asynchronously. Usage: 
    /// <code>
    ///     await using (var handle = await myLock.TryAcquireAsync(...))
    ///     {
    ///         if (handle != null) { /* we have the lock! */ }
    ///     }
    ///     // dispose releases the lock if we took it
    /// </code>
    /// </summary>
    /// <param name="timeout">How long to wait before giving up on the acquisition attempt. Defaults to 0</param>
    /// <param name="cancellationToken">Specifies a token by which the wait can be canceled</param>
    /// <returns>An <see cref="IDistributedSynchronizationHandle"/> which can be used to release the lock or null on failure</returns>
    ValueTask<IDistributedSynchronizationHandle?> TryAcquireAsync(TimeSpan timeout = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Acquires the lock asynchronously, failing with <see cref="TimeoutException"/> if the attempt times out. Usage: 
    /// <code>
    ///     await using (await myLock.AcquireAsync(...))
    ///     {
    ///         /* we have the lock! */
    ///     }
    ///     // dispose releases the lock
    /// </code>
    /// </summary>
    /// <param name="timeout">How long to wait before giving up on the acquisition attempt. Defaults to <see cref="Timeout.InfiniteTimeSpan"/></param>
    /// <param name="cancellationToken">Specifies a token by which the wait can be canceled</param>
    /// <returns>An <see cref="IDistributedSynchronizationHandle"/> which can be used to release the lock</returns>
    ValueTask<IDistributedSynchronizationHandle> AcquireAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);
}