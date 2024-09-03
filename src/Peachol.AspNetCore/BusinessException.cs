namespace System;

public class BusinessException(string message) : Exception(message)
{
    public static void NullOrEmpty(string prefix = "")
        => Throw($"{prefix}不能为空");

    public static void NotUpload(string prefix = "")
        => Throw($"{prefix}未上传");

    public static void Exists(string prefix = "")
        => Throw($"{prefix}数据已存在");

    public static void NotExists(string prefix = "")
        => Throw($"{prefix}数据不存在");

    public static void NotFound(string prefix = "")
        => Throw($"{prefix}数据未找到");

    public static BusinessException Throw(string message) => new(message);
}