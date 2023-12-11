using System.Collections;
using System.IO.Compression;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace BloomFilter.Serialization;

/// <summary>
/// A bit array data transfer object.
/// </summary>
internal class BitArrayDto
{
    private const int CompressLength = 257;

    /// <summary>
    /// Initializes a new instance of the <see cref="BitArrayDto"/> class.
    /// </summary>
    public BitArrayDto()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="BitArrayDto"/> class.
    /// </summary>
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    /// <param name="ba">The ba.</param>
    public BitArrayDto(BitArray ba)
    {
        ArgumentNullException.ThrowIfNull(ba, nameof(ba));

        L = ba.Length;
        var value = ba.BitArrayToByteArray();

        if (L >= CompressLength)
        {
            var ros = new ReadOnlySpan<byte>(value);
            int maxLength = BrotliEncoder.GetMaxCompressedLength(value.Length);
            var s = new Span<byte>(new byte[maxLength]);
            if (!BrotliEncoder.TryCompress(ros, s, out int bytesWritten, 11, 24))
                throw new Exception("Cannot compress!");
            B = s.Slice(0, bytesWritten).ToArray();
        }
        else
        {
            B = value;
        }
    }

    /// <summary>
    /// Property holding actual bit values.
    /// </summary>
    [JsonProperty(PropertyName = "b")]
    [JsonPropertyName("b")]
    public byte[] B { get; set; }

    /// <summary>
    /// Property holding number of elements in the bit array.
    /// </summary>
    [JsonProperty(PropertyName = "l")]
    [JsonPropertyName("l")]
    public int L { get; set; }

    /// <summary>
    /// Converts this data transfer object to a bit array.
    /// </summary>
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    /// <returns>
    /// Bit array object.
    /// </returns>
    public BitArray AsBitArray()
    {
        byte[] bytes;

        if (L >= CompressLength)
        {
            var ros = new ReadOnlySpan<byte>(B);
            var s = new Span<byte>(new byte[L.ByteArrayLength()]);

            if (!BrotliDecoder.TryDecompress(ros, s, out int bytesWritten))
                throw new Exception("Unable to decompress.");

            bytes = s.Slice(0, bytesWritten).ToArray();
        }
        else
        {
            bytes = B;
        }

        return new BitArray(bytes) { Length = L };
    }
}