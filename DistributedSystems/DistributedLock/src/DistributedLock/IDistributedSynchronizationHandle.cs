namespace DistributedLock;

/// <summary>
/// A handle to a distributed lock or other synchronization primitive. To unlock/release,
/// simply dispose the handle.
/// </summary>
public interface IDistributedSynchronizationHandle
    : IDisposable, IAsyncDisposable
{
}