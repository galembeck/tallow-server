using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.Utils;

/// <summary>
/// Conversor JSON que aceita valores como String ou Number e sempre converte para String.
/// Necessário para APIs inconsistentes como SuperFrete que podem retornar price como "15.50" ou 15.50
/// </summary>
public class FlexibleStringConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                return reader.GetString() ?? string.Empty;

            case JsonTokenType.Number:
                if (reader.TryGetDecimal(out var decimalValue))
                    return decimalValue.ToString(System.Globalization.CultureInfo.InvariantCulture);

                if (reader.TryGetDouble(out var doubleValue))
                    return doubleValue.ToString(System.Globalization.CultureInfo.InvariantCulture);

                if (reader.TryGetInt32(out var intValue))
                    return intValue.ToString();

                if (reader.TryGetInt64(out var longValue))
                    return longValue.ToString();

                break;

            case JsonTokenType.True:
                return "true";

            case JsonTokenType.False:
                return "false";

            case JsonTokenType.Null:
                return string.Empty;
        }

        return string.Empty;
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        if (string.IsNullOrEmpty(value))
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(value);
        }
    }
}