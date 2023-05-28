using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
    public DbSet<Cost> Costs { get; set; } = default!;

    public DataContext(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var sqlOptions = optionsBuilder.UseSqlServer(
            @"Server=.\SQLEXPRESS;Database=Hypernova;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True;"
        //,options => options.EnableRetryOnFailure()
        );
        optionsBuilder
            .UseLoggerFactory(_loggerFactory)
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<Claim>().HasQueryFilter(x => x.DeletedAt == null);
        modelBuilder.Entity<Debtor>().HasQueryFilter(x => x.DeletedAt == null);

        foreach (var e in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var i in e.GetProperties().Where(x => x.ClrType == typeof(string)))
            {
                if (i.Name == "Memo")
                {
                    i.SetMaxLength(512);
                }
                else if (i.Name == "Description")
                {
                    i.SetMaxLength(256);
                }
                else if (i.Name.Contains("Name"))
                {
                    i.SetMaxLength(128);
                }
            }
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<ClaimId>().HaveConversion<ClaimId.EfCoreValueConverter>();
        configurationBuilder.Properties<DebtorId>().HaveConversion<DebtorId.EfCoreValueConverter>();
        configurationBuilder
            .Properties<CreditorId>()
            .HaveConversion<CreditorId.EfCoreValueConverter>();
        configurationBuilder.Properties<CostId>().HaveConversion<CostId.EfCoreValueConverter>();
        //configurationBuilder.Properties<PositiveAmount>().HaveConversion<PositiveAmountConverter>();
        configurationBuilder
            .Properties<PositiveAmount>()
            .HaveConversion<AmountToUnsignedDecimalConverter<PositiveAmount>>()
            .HavePrecision(18, 4);
        configurationBuilder
            .Properties<NegativeAmount>()
            .HaveConversion<AmountToUnsignedDecimalConverter<NegativeAmount>>()
            .HavePrecision(18, 4);
        configurationBuilder.Properties<decimal>().HavePrecision(18, 4);
        configurationBuilder.Properties<string>().HaveMaxLength(64);
    }
}
