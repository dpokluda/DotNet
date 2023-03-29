namespace Sharding;

/// <summary>
/// An abstract base class for all hash functions.
/// </summary>
public abstract class HashFunction
{
    /// <summary>
    /// Compute hash value for the specified data.
    /// </summary>
    /// <param name="data">The data to compute hash for.</param>
    /// <returns>
    /// Hash value.
    /// </returns>
    public abstract int ComputeHash(byte[] data);

    /// <summary>
    /// Update value by left rotation.
    /// </summary>
    /// <param name="original">The value.</param>
    /// <param name="bits">The number of bits to rotate.</param>
    /// <returns>
    /// Updated value.
    /// </returns>
    public static uint RotateLeft(uint original, int bits)
    {
        return (original << bits) | (original >> (32 - bits));
    }

    /// <summary>
    /// Update value by moving right.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="pos">The number of bits to move.</param>
    /// <returns>
    /// Updated value.
    /// </returns>
    public static int RightMove(int value, int pos)
    {
        if (pos != 0)
        {
            var mask = 0x7fffffff;
            value >>= 1;
            value &= mask;
            value >>= pos - 1;
        }

        return value;
    }

    /// <summary>
    /// Update value by moving right.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="pos">The number of bits to move.</param>
    /// <returns>
    /// Updated value.
    /// </returns>
    public static long RightMove(long value, int pos)
    {
        if (pos != 0)
        {
            var mask = 0x7fffffff;
            value >>= 1;
            value &= mask;
            value >>= pos - 1;
        }

        return value;
    }
}