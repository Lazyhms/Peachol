using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF.xUnit;

/// <inheritdoc/>
public class ApplicationContext : DbContext
{
    public ApplicationContext()
    {
        Database.EnsureCreated();
    }

    public ApplicationContext(DbContextOptions<ApplicationContext> dbContextOptions) : base(dbContextOptions)
    {
        Database.EnsureCreated();
    }

    /// <inheritdoc/>
    public override int SaveChanges()
    {
        throw new NotImplementedException("Don not call SaveChanges, please call SaveChangesAsync instead.");
    }

    /// <inheritdoc/>
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        throw new NotImplementedException("Don not call SaveChanges, please call SaveChangesAsync instead.");
    }

    /// <inheritdoc/>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => base.SaveChangesAsync(cancellationToken);

    /// <inheritdoc/>
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        => base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyEntitiesFromAssembly(typeof(EntityBase).Assembly, w => typeof(EntityBase).IsAssignableFrom(w) && !w.IsAbstract);
    }

    private readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(l => l.AddDebug());

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var connectionString = "server=127.0.0.1;userid=root;pwd=root;port=3306;database=SmartConstruction;sslmode=none;Charset=utf8;AutoEnlist=false";
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        optionsBuilder.UseSoftDelete();
        optionsBuilder.IncludeXmlComments();
        optionsBuilder.EnableRemoveForeignKey();
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseLoggerFactory(_loggerFactory);
    }
}

/// <summary>
/// 学校
/// </summary>
[Table("to.school")]
[SoftDelete("Deleted")]
public class School : EntityBase
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 地址
    /// </summary>
    public string Address { get; set; } = default!;

    /// <summary>
    /// 年龄
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// 是否删除
    /// </summary>
    public bool Deleted { get; set; }
}

/// <summary>
/// 学生
/// </summary>
[Table("to.stu")]
public class Stu : EntityBase
{

    public long SchoolId { get; set; }


    public School School { get; set; } = default!;

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 生日
    /// </summary>
    public string BirthDay { get; set; } = default!;
}

/// <summary>
/// 
/// </summary>
public abstract class EntityBase
{
    /// <summary>
    /// 
    /// </summary>
    [Column(Order = 1)]
    public long Id { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    [UpdateIgnore]
    [Column(Order = 27)]
    public long Creator { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [UpdateIgnore]
    [Column(Order = 28)]
    public DateTime Created { get; private set; } = DateTime.Now;

    /// <summary>
    /// 更新人
    /// </summary>
    [AddIgnore]
    [Column(Order = 29)]
    public long? Updater { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [AddIgnore]
    [Column(Order = 30)]
    public DateTime? Updated { get; private set; }
}