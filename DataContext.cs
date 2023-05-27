using StronglyTypedIds;

[assembly: StronglyTypedIdDefaults(
    backingType: StronglyTypedIdBackingType.Guid,
    converters: StronglyTypedIdConverter.EfCoreValueConverter
        | StronglyTypedIdConverter.SystemTextJson
)]

public class DataContext : DbContext
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
        optionsBuilder.UseSqlServer(
            @"Server=.\SQLEXPRESS;Database=Hypernova;Trusted_Connection=True;TrustServerCertificate=True;"
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
