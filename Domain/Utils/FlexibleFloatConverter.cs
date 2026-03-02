using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.Utils;

/// <summary>
/// Conversor JSON que aceita valores como String ou Number e sempre converte para float.
/// Necessário para APIs inconsistentes como SuperFrete.
/// </summary>
public class FlexibleFloatConverter : JsonConverter<float>
{
    public override float Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Number:
                if (reader.TryGetSingle(out var floatValue))
                    return floatValue;

                if (reader.TryGetDouble(out var doubleValue))
                    return (float)doubleValue;

                if (reader.TryGetDecimal(out var decimalValue))
                    return (float)decimalValue;

                if (reader.TryGetInt32(out var intValue))
                    return intValue;

                if (reader.TryGetInt64(out var longValue))
                    return longValue;

                break;

            case JsonTokenType.String:
                var stringValue = reader.GetString();
                if (string.IsNullOrWhiteSpace(stringValue))
                    return 0f;

                stringValue = stringValue.Trim().Replace(",", ".");

                if (float.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                    return result;

                break;

            case JsonTokenType.Null:
                return 0f;
        }

        return 0f;
    }

    public override void Write(Utf8JsonWriter writer, float value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}