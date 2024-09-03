namespace System;

public static class EnumExtensions
{
    public static TAttribute? GetAttributeOfType<TAttribute>(this Enum value) where TAttribute : Attribute
        => value.GetType().GetField(value.ToString())?.GetCustomAttribute<TAttribute>();

    public static string GetDescription(this Enum value)
        => value.GetAttributeOfType<DescriptionAttribute>()?.Description ?? string.Empty;
}