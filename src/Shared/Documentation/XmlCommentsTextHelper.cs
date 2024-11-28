using System.Net;
using System.Text.RegularExpressions;

namespace System.Xml.XPath;

internal static partial class XmlCommentsTextHelper
{

#if NET8_0_OR_GREATER

    [GeneratedRegex(@"<(see|paramref) (name|cref|langword)=""([TPF]{1}:)?(?<display>.+?)"" ?/>")]
    private static partial Regex RefTagPattern();

    [GeneratedRegex(@"<c>(?<display>.+?)</c>")]
    private static partial Regex CodeTagPattern();

    [GeneratedRegex(@"<code>(?<display>.+?)</code>", RegexOptions.Singleline)]
    private static partial Regex MultilineCodeTagPattern();

    [GeneratedRegex(@"<para>(?<display>.+?)</para>", RegexOptions.Singleline)]
    private static partial Regex ParaTagPattern();

    [GeneratedRegex(@"<see href=\""(.*)\"">(.*)<\/see>")]
    private static partial Regex HrefPattern();

#elif NET6_0_OR_GREATER

    private static Regex RefTagPattern() => new(@"<(see|paramref) (name|cref|langword)=""([TPF]{1}:)?(?<display>.+?)"" ?/>");

    private static Regex CodeTagPattern() => new(@"<c>(?<display>.+?)</c>");

    private static Regex MultilineCodeTagPattern() => new(@"<code>(?<display>.+?)</code>", RegexOptions.Singleline);

    private static Regex ParaTagPattern() => new(@"<para>(?<display>.+?)</para>", RegexOptions.Singleline);

    private static Regex HrefPattern() => new(@"<see href=\""(.*)\"">(.*)<\/see>");

#endif

    public static string Humanize(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        return text
            .NormalizeIndentation()
            .HumanizeRefTags()
            .HumanizeHrefTags()
            .HumanizeCodeTags()
            .HumanizeMultilineCodeTags()
            .HumanizeParaTags()
            .DecodeXml();
    }

    private static string NormalizeIndentation(this string text)
    {
        string[] lines = text.Split('\n');
        string? padding = GetCommonLeadingWhitespace(lines);

        int padLen = padding == null ? 0 : padding.Length;

        for (int i = 0, l = lines.Length; i < l; ++i)
        {
            string line = lines[i].TrimEnd('\r');

            if (padLen != 0 && line.Length >= padLen && line.Substring(0, padLen) == padding)
                line = line.Substring(padLen);

            lines[i] = line;
        }

        return string.Join("\r\n", lines.SkipWhile(string.IsNullOrWhiteSpace)).TrimEnd();
    }

    private static string? GetCommonLeadingWhitespace(string[] lines)
    {
        ArgumentNullException.ThrowIfNull(nameof(lines));

        if (lines.Length == 0)
            return null;

        string[] nonEmptyLines = lines
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

        if (nonEmptyLines.Length < 1)
            return null;

        int padLen = 0;

        string seed = nonEmptyLines[0];
        for (int i = 0, l = seed.Length; i < l; ++i)
        {
            if (!char.IsWhiteSpace(seed, i))
                break;

            if (nonEmptyLines.Any(line => line[i] != seed[i]))
                break;

            ++padLen;
        }

        if (padLen > 0)
            return seed.Substring(0, padLen);

        return null;
    }

    private static string HumanizeRefTags(this string text)
    {
        return RefTagPattern().Replace(text, (match) => match.Groups["display"].Value);
    }

    private static string HumanizeHrefTags(this string text)
    {
        return HrefPattern().Replace(text, m => $"[{m.Groups[2].Value}]({m.Groups[1].Value})");
    }

    private static string HumanizeCodeTags(this string text)
    {
        return CodeTagPattern().Replace(text, (match) => "`" + match.Groups["display"].Value + "`");
    }

    private static string HumanizeMultilineCodeTags(this string text)
    {
        return MultilineCodeTagPattern().Replace(text, (match) => "```" + match.Groups["display"].Value + "```");
    }

    private static string HumanizeParaTags(this string text)
    {
        return ParaTagPattern().Replace(text, (match) => "<br>" + match.Groups["display"].Value);
    }

    private static string DecodeXml(this string text)
    {
        return WebUtility.HtmlDecode(text);
    }
}