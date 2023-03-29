using System.Collections;
using Newtonsoft.Json;

namespace BloomFilter.Serialization;

/// <summary>
/// Json bit array converter.
/// </summary>
/// <seealso cref="JsonConverter{BitArray}"/>
public class BitArrayConverter : JsonConverter<BitArray>
{
    /// <summary>
    /// Reads the JSON representation of the object.
    /// </summary>
    /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="existingValue">The existing value of object being read. If there is no existing value then <c>null</c> will be used.</param>
    /// <param name="hasExistingValue">The existing value has a value.</param>
    /// <param name="serializer">The calling serializer.</param>
    /// <returns>
    /// The object value.
    /// </returns>
    /// <seealso cref="JsonConverter{BitArray}.ReadJson(JsonReader,Type,BitArray,bool,JsonSerializer)"/>
    public override BitArray ReadJson(JsonReader reader, Type objectType, BitArray existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var dto = serializer.Deserialize<BitArrayDto>(reader);
        if (dto == null)
            return null;
        var bitArray = dto.AsBitArray();
        return bitArray;
    }

    /// <summary>
    /// Writes the JSON representation of the object.
    /// </summary>
    /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
    /// <param name="value">The value.</param>
    /// <param name="serializer">The calling serializer.</param>
    /// <seealso cref="JsonConverter{BitArray}.WriteJson(JsonWriter,BitArray,JsonSerializer)"/>
    public override void WriteJson(JsonWriter writer, BitArray value, JsonSerializer serializer)
    {
        var dto = new BitArrayDto(value);
        serializer.Serialize(writer, dto);
    }
}