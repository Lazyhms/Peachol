namespace System.Xml.XPath;

internal class XmlDocumentationComments(XPathDocument xPathDocument)
{
    private const string AssemblyXPath = "/doc/assembly[name='{0}']";
    private const string SummaryXPath = "/doc/members/member[@name='{0}']/summary";

    private readonly XPathNavigator _xmlNavigator = xPathDocument.CreateNavigator();

    public XPathNavigator? FindAssemblyXPathNavigatoryForType(Type type)
        => _xmlNavigator.SelectSingleNode(string.Format(AssemblyXPath, type.Assembly.GetName().Name));

    public string GetMemberNameForType(Type type)
    {
        var memberXPath = XmlCommentsNodeNameHelper.GetMemberNameForType(type);
        if (memberXPath is null)
        {
            return string.Empty;
        }

        var xPathNavigator = _xmlNavigator.SelectSingleNode(string.Format(SummaryXPath, memberXPath));
        if (xPathNavigator is null || string.IsNullOrWhiteSpace(xPathNavigator.Value))
        {
            return string.Empty;
        }

        return XmlCommentsTextHelper.Humanize(xPathNavigator.Value);
    }

    public string GetMemberNameForFieldOrProperty(MemberInfo memberInfo)
    {
        var memberXPath = XmlCommentsNodeNameHelper.GetMemberNameForFieldOrProperty(memberInfo);
        if (memberXPath is null)
        {
            return string.Empty;
        }

        var xPathNavigator = _xmlNavigator.SelectSingleNode(string.Format(SummaryXPath, memberXPath));
        if (xPathNavigator is null || string.IsNullOrWhiteSpace(xPathNavigator.Value))
        {
            return string.Empty;
        }

        return XmlCommentsTextHelper.Humanize(xPathNavigator.Value);
    }
}
