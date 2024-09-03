namespace System.Text.Json.Serialization;

[Flags]
public enum GuidConverterOptions
{
    /// <summary>
    /// 00000000000000000000000000000000
    /// </summary>
    N = 0x1,

    /// <summary>
    /// 00000000-0000-0000-0000-000000000000
    /// </summary>
    D = 0x2,

    /// <summary>
    /// {00000000-0000-0000-0000-000000000000}
    /// </summary>
    B = 0x4,

    /// <summary>
    /// (00000000-0000-0000-0000-000000000000)
    /// </summary>
    P = 0x8,

    /// <summary>
    /// {0x00000000，0x0000，0x0000，{0x00，0x00，0x00，0x00，0x00，0x00，0x00，0x00}}
    /// </summary>
    X = 0xA,
}