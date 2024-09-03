namespace System.Text.Json.Serialization;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class JsonGuidConverterAttribute(GuidConverterOptions? guidConverterOptions) : JsonConverterAttribute
{
    public JsonGuidConverterAttribute() : this(null)
    {
    }

    public override JsonConverter? CreateConverter(Type typeToConvert)
        => new JsonGuidConverter(guidConverterOptions);
}