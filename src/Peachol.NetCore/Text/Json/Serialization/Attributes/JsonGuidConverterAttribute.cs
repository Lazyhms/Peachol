namespace System.Text.Json.Serialization;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public sealed class JsonGuidConverterAttribute(JsonGuidHandling? jsonGuidHandling) : JsonConverterAttribute
{
    public JsonGuidConverterAttribute() : this(null)
    {
    }

    public override JsonConverter? CreateConverter(Type typeToConvert)
        => new JsonGuidConverter(jsonGuidHandling);
}