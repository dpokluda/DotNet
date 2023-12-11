using System.Collections;
using System.Text;
using Newtonsoft.Json;

namespace BloomFilter.Serialization;

/// <summary>
/// Helper converter that converts bit array to string value where each bit is representing as either <c>1</c> (when <c>true</c>) or <c>0</c> (when <c>false</c>).
/// </summary>
/// <seealso cref="JsonConverter{BitArray}"/>
public class BitArrayToStringConverter : JsonConverter<BitArray>
{
    /// <summary>
    /// Reads the JSON representation of the object.
    /// </summary>
    /// <exception cref="JsonSerializationException">Thrown when a JSON Serialization error condition occurs.</exception>
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
        if (reader.TokenType == JsonToken.Null)
            return null;
        else if (reader.TokenType != JsonToken.String)
            throw new JsonSerializationException(string.Format("Unexpected token {0}", reader.TokenType));
        var s = (string)reader.Value;
        var bitArray = new BitArray(s.Length);
        for (int i = 0; i < s.Length; i++)
            bitArray[i] = s[i] == '0' ? false : s[i] == '1' ? true : throw new JsonSerializationException(string.Format("Unknown bit value {0}", s[i]));
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
        writer.WriteValue(value.Cast<bool>().Aggregate(new StringBuilder(value.Length), (sb, b) => sb.Append(b ? "1" : "0")).ToString());
    }
}