namespace EF.xUnit;
using Microsoft.EntityFrameworkCore;

public class DbSetTests
{
    private readonly ApplicationContext _context = new();

    [Fact]
    public async Task Add()
    {
        _context.Set<School>().Add(new School
        {
            Id = 6,
            Name = "name6",
            Address = "address2",
            Age = 0,
        });

        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task EUpdate()
    {
        await _context.Set<School>().ExecuteSoftRemoveAsync();
    }

    [Fact]
    public async Task Query()
    {
        var t = await _context.Set<School>().ToListAsync();
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

        _context.Set<School>().Update(new Dictionary<string, object?>
        {
            { "Id" , 1L},
            {"Name","name4" },
            {"Address","address4"},
            {"Age",0},
        });

        await _context.SaveChangesAsync();

        _context.Set<School>().Update(new
        {
            Id = 1L,
            Name = "name5",
            Address = "address5",
            Age = 0,
        }, i => new { i.Name });

        await _context.SaveChangesAsync();

        _context.Set<School>().Update(new Dictionary<string, object?>
        {
            { "Id" , 1L},
            {"Name","name6" },
            {"Address","address6"},
            {"Age",0 },
        }, i => new { i.Address });

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
        _context.Set<School>().Remove(1L);

        await _context.SaveChangesAsync();

        _context.Set<School>().Remove(2L);

        await _context.SaveChangesAsync();
    }
}
