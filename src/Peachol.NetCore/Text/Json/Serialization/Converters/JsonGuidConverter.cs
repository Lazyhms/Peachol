namespace System.Text.Json.Serialization;

public sealed class JsonGuidConverter(JsonGuidHandling? jsonGuidHandling) : JsonConverter<Guid>
{
    private readonly JsonConverter<Guid> s_defaultConverter =
        (JsonConverter<Guid>)JsonSerializerOptions.Default.GetConverter(typeof(Guid));

    public JsonGuidConverter() : this(null)
    {
    }

    public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => s_defaultConverter.Read(ref reader, typeToConvert, options);

    public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
    {
        switch (jsonGuidHandling)
        {
            case JsonGuidHandling.N:
                writer.WriteStringValue(value.ToString("N"));
                break;
            case JsonGuidHandling.D:
                writer.WriteStringValue(value.ToString("D"));
                break;
            case JsonGuidHandling.B:
                writer.WriteStringValue(value.ToString("B"));
                break;
            case JsonGuidHandling.P:
                writer.WriteStringValue(value.ToString("P"));
                break;
            case JsonGuidHandling.X:
                writer.WriteStringValue(value.ToString("X"));
                break;
            default:
                s_defaultConverter.Write(writer, value, options);
                break;
        }
    }
}