namespace BloomFilter.Serialization;

/// <summary>
/// BitArray extension methods.
/// </summary>
internal static class BitArrayExtensions
{
    /// <summary>
    /// Convert bit array to byte array.
    /// </summary>
    /// <param name="bits">The bit array.</param>
    /// <returns>
    /// Byte array.
    /// </returns>
    public static byte[] BitArrayToByteArray(this System.Collections.BitArray bits)
    {
        byte[] ret = new byte[bits.ByteArrayLength()];
        bits.CopyTo(ret, 0);
        return ret;
    }

    /// <summary>
    /// Number of bytes necessary to hold the bit array.
    /// </summary>
    /// <param name="bits">The bit array.</param>
    /// <returns>
    /// Integer value representing the number of bytes to hold the bit array value.
    /// </returns>
    public static int ByteArrayLength(this System.Collections.BitArray bits)
    {
        return (bits.Length - 1) / 8 + 1;
    }

    /// <summary>
    /// Number of bytes necessary to hold the bit array.
    /// </summary>
    /// <param name="numberOfBits">The number of bits.</param>
    /// <returns>
    /// Integer value representing the number of bytes to hold the bit array value.
    /// </returns>
    public static int ByteArrayLength(this int numberOfBits)
    {
        return (numberOfBits - 1) / 8 + 1;
    }
}