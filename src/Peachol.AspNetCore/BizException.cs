namespace System;

public class BizException(string message) : Exception(message)
{
    public static void Throw(string message) => throw new(message);
}