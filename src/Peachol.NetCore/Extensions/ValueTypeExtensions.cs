namespace System;

public static class ValueTypeExtensions
{
    public static string PadLeft(this ValueType value, int totalWidth, char paddingChar)
        => value.ToString()?.PadLeft(totalWidth, paddingChar) ?? string.Empty;

    public static string PadRight(this ValueType value, int totalWidth, char paddingChar)
        => value.ToString()?.PadRight(totalWidth, paddingChar) ?? string.Empty;
}
