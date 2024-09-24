using System.Text.Json;
using System.Text.Json.Nodes;

namespace Peachol.xUnit;

public class UnitTest1
{
    private readonly JsonDocument _document = JsonDocument.Parse(JsonSerializer.Serialize(new { Id = 1, Name = "我的" }));
    private readonly JsonNode _node = JsonNode.Parse(JsonSerializer.Serialize(new { Id = 1, Name = "我的" }), new JsonNodeOptions { PropertyNameCaseInsensitive = true })!;

    [Fact]
    public void Test1()
    {
        BizException.Throw("数据");

        var node = _node.AsObject();

        var value = node["Id"]?.AsValue();
    }

    [Fact]
    public void Test2()
    {
        var t = _document.RootElement.GetProperty("Id").GetInt32();
    }

    JsonSerializerOptions _options = new(JsonSerializerDefaults.Web)
    {
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
    };

    [Fact]
    public void Test3()
    {
        var t1 = Tests.A.GetDescription();
        var t3 = Tests.A.GetDescription();

        var t2 = Enum.GetNames<Tests>();

        Test[] t = [new Test { Id = 1, PId = 0, Name = "11" }, new Test { Id = 2, PId = 1, Name = "22" }];

        var tttt = t.ToDictionary(k => k.Id, v => JsonSerializer.SerializeToNode(t));

        var tt = t.ToTreeNode(s => s.Id, p => p.PId).ToList();

        var ttt = JsonSerializer.Serialize(tt, _options);
    }

    public enum Tests
    {
        A,
        B, C, D, E, F,
    }
}

public class Test
{
    public int Id { get; set; }

    public int PId { get; set; }

    public string Name { get; set; }
}