using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Xunit;

namespace AspNetCore.XUnit;

public class UnitTest1
{
    private readonly JsonNode _node = JsonNode.Parse(JsonSerializer.Serialize(new { Id = 1, Name = "我的" }), new JsonNodeOptions { PropertyNameCaseInsensitive = true })!;

    [Fact]
    public void Test1()
    {
        Expression<Func<Test, bool>> expression = t => true;
        Expression<Func<Test, bool>> expression1 = t => false;
        var expression2 = expression.And(x => x.Id == 1);
        var expression3 = expression1.And(x => x.Id == 1);

        var expression4 = expression.Or(x => x.Id == 1);
        var expression5 = expression1.Or(x => x.Id == 1);

        var t = Enumerable.Range(0, 2).Insert(1, 4);

        var t1 = Enumerable.Range(0, 2).Insert(1, Enumerable.Range(4, 3));

        var tt2 = new List<Test> { new() { Id = 1, }, new() { Id = 2, } }.GroupBy(g => g.Id, (r1, r2) => new
        {
            r1,
            Name = r2.Select(s => s.Name)
        });

        var tt3 = new List<Test> { new() { Id = 1, }, new() { Id = 2, }, new() { Id = 1, } }.LastIndexOf(f => f.Id == 1);

        int? i = 1;

        BizException.ThrowIfNull(i, "不能为空");

        var node = _node.AsObject();

        var value = node["Id"]?.AsValue();
    }

    [Fact]
    public void Test3()
    {
        var ttt11 = ((string?)null).IsNullOrWhiteSpace("测试");

        var t13 = new string('0', 5);

        var options = JsonSerializerOptions.Default.ApplyWebDefault();

        var t12 = JsonSerializer.Serialize(Guid.NewGuid(), options);

        var ttt1 = JsonSerializer.Serialize(new Test { Score = 0.00001m }, options);


        var t1 = Tests.A.GetDescription();
        var t3 = Tests.A.GetDescription();

        var t2 = Enum.GetNames<Tests>();

        Test[] t = [new Test { Id = 1, PId = 0, Name = "11" }, new Test { Id = 2, PId = 1, Name = "22" }];

        var tttt = t.ToDictionary(k => k.Id, v => JsonSerializer.SerializeToNode(t));

        var tt = t.ToTreeNode(s => s.Id, p => p.PId).ToList();

        var ttt = JsonSerializer.Serialize(tt, options);
    }


}

public class Test
{
    public int Id { get; set; }

    public int PId { get; set; }

    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public decimal Score { get; set; }

    public string? Name { get; set; } = null;

    [JsonGuidHandling(JsonGuidHandling.Parentheses)]
    public Guid Guid { get; set; }
}


public enum Tests
{
    A,
    B, C, D, E, F,
}
