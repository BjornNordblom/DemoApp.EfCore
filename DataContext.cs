using Microsoft.EntityFrameworkCore.Infrastructure;

[assembly: StronglyTypedIdDefaults(
    backingType: StronglyTypedIdBackingType.Guid,
    converters: StronglyTypedIdConverter.EfCoreValueConverter
        | StronglyTypedIdConverter.SystemTextJson
)]

public interface IDataContext
{
    DatabaseFacade Database { get; }
    DbSet<Claim> Claims { get; set; }
    DbSet<Debtor> Debtors { get; set; }
    DbSet<Creditor> Creditors { get; set; }
    DbSet<ClaimDebtor> ClaimDebtors { get; set; }

    // AddRangeAsync
    Task AddRangeAsync(params object[] entities);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class DataContext : DbContext, IDataContext
{
    private readonly ILoggerFactory _loggerFactory;

    public DbSet<Claim> Claims { get; set; } = default!;
    public DbSet<Debtor> Debtors { get; set; } = default!;
    public DbSet<Creditor> Creditors { get; set; } = default!;
    public DbSet<ClaimDebtor> ClaimDebtors { get; set; } = default!;

    public DataContext(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var sqlOptions = optionsBuilder.UseSqlServer(
            @"Server=.\SQLEXPRESS;Database=Hypernova;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True;",
            options => options.EnableRetryOnFailure()
        );
        optionsBuilder
            .UseLoggerFactory(_loggerFactory)
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<ClaimId>().HaveConversion<ClaimId.EfCoreValueConverter>();
        configurationBuilder.Properties<DebtorId>().HaveConversion<DebtorId.EfCoreValueConverter>();
        configurationBuilder.Properties<decimal>().HavePrecision(18, 4);
    }
}
