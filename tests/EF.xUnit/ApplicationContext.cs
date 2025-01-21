using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations.Schema;

namespace EF.XUnit;

/// <inheritdoc/>
public class ApplicationContext : DbContext
{
    /// <inheritdoc/>
    public ApplicationContext()
    {
        Database.EnsureCreated();
    }

    /// <inheritdoc/>
    public ApplicationContext(DbContextOptions<ApplicationContext> dbContextOptions) : base(dbContextOptions)
    {
        Database.EnsureCreated();
    }

    /// <inheritdoc/>
    public override int SaveChanges()
        => throw new NotImplementedException("Do not call SaveChanges, please call SaveChangesAsync instead.");

    /// <inheritdoc/>
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
        => throw new NotImplementedException("Do not call SaveChanges, please call SaveChangesAsync instead.");

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

        modelBuilder.ApplyEntitiesFromAssembly<EntityBase>(typeof(EntityBase).Assembly, c => c.TenantId == 1, t => t.Domain == 2);
    }

    private readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(l => l.AddDebug());

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        var connectionString = "server=127.0.0.1;userid=root;pwd=root;port=3306;database=SmartConstruction;sslmode=none;Charset=utf8;AutoEnlist=false";
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        optionsBuilder.UsePeachol(optionsBuilder =>
        {
            optionsBuilder.UseSoftDelete();
            optionsBuilder.IncludeXmlComments();
            optionsBuilder.EnableRemoveForeignKey();
        });

        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseLoggerFactory(_loggerFactory);
    }
}

/// <summary>
/// 学校
/// </summary>
[Table("to.school")]
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
    /// 价位
    /// </summary>
    public decimal Price { get; set; }
}

/// <summary>
/// 学生
/// </summary>
[Table("to.stu")]
public class Stu : EntityBase
{
    /// <summary>
    /// 
    /// </summary>
    public long SchoolId { get; set; }

    /// <summary>
    /// 
    /// </summary>
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
/// <param name="tenantId"></param>
/// <param name="domain"></param>
public abstract class EntityBase(int tenantId, int domain)
{
    /// <summary>
    /// 
    /// </summary>
    protected EntityBase() : this(1, 2)
    {
    }

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
    public DateTime? Updated { get; private set; } = DateTime.Now;

    /// <summary>
    /// 
    /// </summary>
    public long TenantId { get; set; } = tenantId;

    /// <summary>
    /// 
    /// </summary>
    public long Domain { get; set; } = domain;
}
