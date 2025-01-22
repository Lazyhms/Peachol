using System.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EF.XUnit;

public class DbSetTests
{
    private readonly ApplicationContext _context = new();

    [Fact]
    public async Task Add()
    {
        var t1 = await _context.Set<School>().ToListAsync();
        var t2 = await _context.Set<School>().IgnoreQueryFilters(s => s.Domain).ToListAsync();
        var t3 = await _context.Set<School>().IgnoreQueryFilters(nameof(School.Name)).ToListAsync();
        var t4 = await _context.Set<School>().IgnoreQueryFilters().ToListAsync();
        var t5 = await _context.Set<School>().OrderBy(nameof(School.Name)).ToListAsync();
        var t6 = await _context.Set<School>().OrderByDescending(nameof(School.Name)).ToListAsync();
        var t7 = await _context.Set<School>().OrderByEFProperty("Deleted").ToListAsync();
        var t8 = await _context.Set<School>().OrderByEFPropertyDescending("Deleted").ToListAsync();

        var b1 = await _context.Set<Stu>().ToListAsync();

        var c1 = await _context.Set<School>().IgnoreQueryFilters().Where(w => true && w.Id > 1).ToListAsync();
        var c3 = await _context.Set<School>().IgnoreQueryFilters().Where(w => false && w.Id > 1).ToListAsync();
        var c2 = await _context.Set<School>().IgnoreQueryFilters().Where(w => true || w.Id > 1).ToListAsync();
        var c4 = await _context.Set<School>().IgnoreQueryFilters().Where(w => false || w.Id > 1).ToListAsync();

        _context.Set<School>().Remove(1L);

        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task EUpdate()
    {
        await _context.Set<School>().IgnoreQueryFilters("Name").ExecuteDeleteOrSoftDeleteAsync();
        await _context.Set<School>().IgnoreQueryFilters("Name").ExecuteDeleteOrSoftDeleteAsync();
    }

    [Fact]
    public async Task Query()
    {
        var tt1 = await _context.Database.ExecuteSqlQueryAsync($"select * from smartconstruction.`to.school`", r => new
        {
            Name4 = r[0],
            Name0 = r["Name"],
            Name1 = r.GetValue<string>("Name"),
            Name3 = r.GetValueOrDefault<string>("Name"),
            Name2 = r.GetValueOrDefault("Name", string.Empty),
        });
    }

    [Fact]
    public async Task AddOrUpdate()
    {
        _context.Set<School>().AddOrUpdate(new
        {
            Id = 1L,
            Name = "name3",
            Address = "address3",
            Age = 0,
        });

        await _context.SaveChangesAsync();

        _context.Set<School>().AddOrUpdate(new School
        {
            Id = 2L,
            Name = "name4",
            Address = "address4",
            Age = 0,
        });

        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task Update()
    {
        _context.Set<School>().Update(new School
        {
            Id = 1L,
            Name = "name3",
            Address = "address3",
            Age = 0,
        });

        await _context.SaveChangesAsync();

        _context.Set<School>().Update(new
        {
            Id = 1L,
            Name = "name3",
            Address = "address3",
            Age = 0,
        });

        await _context.SaveChangesAsync();

        _context.Set<School>().Update(new Dictionary<string, object>
        {
            { "Id" , 1L },
            { "Name" , "name4" },
            { "Address" , "address4" },
            { "Age" , 0 },
        });

        await _context.SaveChangesAsync();

        _context.Set<School>().Update(new
        {
            Id = 1L,
            Name = "name5",
            Address = "address5",
            Age = 0,
        }).IngoreProperty(s => new { s.Name });

        await _context.SaveChangesAsync();

        _context.Set<School>().Update(new Dictionary<string, object>
        {
            { "Id" , 1L},
            { "Name" , "name6" },
            { "Address" , "address6" },
            { "Age" , 0 },
        }).UpdateProperty(s => new { s.Address });

        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task SoftRemove()
    {
        _context.Set<School>().SoftRemove(new School
        {
            Id = 1,
            Name = "name2",
            Address = "address2",
            Age = 0,
        });

        await _context.SaveChangesAsync();

        _context.Set<School>().SoftRemove(2L);

        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task Remove()
    {
        var t = new School
        {
            Id = 6L,
            Name = "111",
            Address = "aa",
            Age = 1
        };
        _context.Set<School>().Add(t);

        await _context.SaveChangesAsync();

        _context.Set<School>().Remove(t.Id);

        await _context.SaveChangesAsync();
    }
}
