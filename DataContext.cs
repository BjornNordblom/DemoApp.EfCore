using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

public class DataContext : DbContext
{
    private readonly ILoggerFactory _loggerFactory;

    public DbSet<Claim> Claims { get; set; }

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
            .UseLoggerFactory(
                _loggerFactory
            // LoggerFactory.Create(builder =>
            // {
            //     builder
            //         .AddFilter(
            //             (category, level) =>
            //                 category == DbLoggerCategory.Database.Command.Name
            //                 && level == LogLevel.Information
            //         )
            //         .AddConsole();
            // })
            )
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging();

        // LoggerFactory.Create(builder =>
        // {
        //     builder
        //         .AddSimpleConsole(
        //             (options) =>
        //             {
        //                 options.ColorBehavior = LoggerColorBehavior.Enabled;
        //                 options.TimestampFormat = "[HH:mm:ss] ";
        //             }
        //         )
        //         .AddFilter(
        //             (category, level) =>
        //                 category == DbLoggerCategory.Database.Command.Name
        //                 && level == LogLevel.Debug
        //         );
        // })
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<ClaimId>().HaveConversion<ShortIdConverter<ClaimId>>();
        configurationBuilder.Properties<decimal>().HavePrecision(18, 4);
    }
}
