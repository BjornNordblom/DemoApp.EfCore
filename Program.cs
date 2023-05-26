using Microsoft.EntityFrameworkCore;
using Serilog;

public static class Program
{
    private static ILoggerFactory _logFactory;

    private async static Task<DbContext> CreateDatabase()
    {
        var dataContext = new DataContext(_logFactory);
        await dataContext.Database.EnsureDeletedAsync();
        await dataContext.Database.EnsureCreatedAsync();
        return dataContext;
    }

    private async static Task SeedDatabase(DbContext dataContext)
    {
        var creditor = new Creditor { Id = Guid.NewGuid(), Name = "ABC Company" };

        // Create a new debtor of type NaturalPerson
        var debtor = new Debtor
        {
            Id = Guid.NewGuid(),
            Type = Debtor.DebtorType.NaturalPerson,
            NaturalPerson = new DebtorNaturalPerson
            {
                FirstName = "John",
                LastName = "Doe",
                PersonalNumber = "121212-1212"
            }
        };

        var debtor2 = new Debtor
        {
            Id = Guid.NewGuid(),
            Type = Debtor.DebtorType.NaturalPerson,
            NaturalPerson = new DebtorNaturalPerson
            {
                FirstName = "John",
                LastName = "Doe",
                PersonalNumber = "121212-1212"
            }
        };

        var claim = Claim.Create("ABC123", creditor.Id);
        var claim2 = Claim.Create("ABC123", creditor.Id);

        var claimDebtor = new ClaimDebtor
        {
            Debtor = debtor,
            Claim = claim,
            Involvement = ClaimDebtor.DebtorInvolvement.Primary
        };
        var claimDebtor2 = new ClaimDebtor
        {
            Debtor = debtor,
            Claim = claim2,
            Involvement = ClaimDebtor.DebtorInvolvement.Primary
        };
        var claimDebtor3 = new ClaimDebtor
        {
            Debtor = debtor2,
            Claim = claim,
            Involvement = ClaimDebtor.DebtorInvolvement.Primary
        };
        var claimInvoice = new Invoice
        {
            ClaimItem = new ClaimItem()
            {
                Claim = claim,
                ReferenceNo = "INV123",
                Type = ClaimItem.ClaimType.Invoice
            },
            Amount = 1000
        };
        var claimInvoice2 = new Invoice
        {
            ClaimItem = new ClaimItem()
            {
                Claim = claim2,
                ReferenceNo = "INV123",
                Type = ClaimItem.ClaimType.Invoice
            },
            Amount = 1000
        };

        var claimCreditInvoice = new CreditInvoice
        {
            ClaimItem = new ClaimItem()
            {
                Claim = claim,
                ReferenceNo = "CRED-INV123",
                Type = ClaimItem.ClaimType.CreditNote
            },
            CreditedInvoice = claimInvoice,
            CreditedAmount = 500
        };

        await dataContext.AddRangeAsync(
            creditor,
            debtor,
            debtor2,
            claim,
            claimDebtor,
            claimDebtor2,
            claimDebtor3,
            claimInvoice,
            claimInvoice2,
            claimCreditInvoice
        );
        await dataContext.SaveChangesAsync();
    }

    public static async Task Main()
    {
        Log.Logger = new LoggerConfiguration().Enrich
            .FromLogContext()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
            )
            .CreateLogger();
        ;

        _logFactory = new LoggerFactory().AddSerilog(Log.Logger);
        try
        {
            var dataContext = CreateDatabase().Result;
            SeedDatabase(dataContext).Wait();
            var claimId = PrintContent();
            PrintOne(claimId);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void PrintOne(ClaimId id)
    {
        Log.Warning("PrintOne");
        Log.Information("Claim: {id}", id);

        using var dataContext = new DataContext(_logFactory);
        var debtors = dataContext.Debtors.Include(x => x.Claims.Where(c => c.Id == id)).ToList();
        foreach (var debtor in debtors)
        {
            Log.Information("Found debtor: {debtorId}", debtor.Id);
            foreach (var claim in debtor.Claims)
            {
                Log.Information(
                    "Found claim: {claimId} {claimReferenceNo}",
                    claim.Id,
                    claim.ReferenceNo
                );
            }
        }
        var claims = dataContext.Claims.Include(x => x.Debtors).Where(x => x.Id == id).ToList();
        foreach (var claim in claims)
        {
            Log.Information(
                "Found claim: {claimId} {claimReferenceNo}",
                claim.Id,
                claim.ReferenceNo
            );
            foreach (var debtor in claim.Debtors)
            {
                Log.Information("Found debtor: {debtorId}", debtor.Id);
            }
        }
        // var claims = dataContext.Claims.Include(x => x.Debtors).Where(x => x.Id == id).ToList();
        // foreach (var claim in claims)
        // {
        //     Log.Information(
        //         "Found claim: {claimId} {claimReferenceNo}",
        //         claim.Id,
        //         claim.ReferenceNo
        //     );
        //     foreach (var debtor in claim.Debtors)
        //     {
        //         Log.Information("Found debtor: {debtorId}", debtor.Id);
        //     }
        // }
    }

    private static ClaimId PrintContent()
    {
        Log.Warning("PrintContent");

        using var dataContext = new DataContext(_logFactory);
        var claims = dataContext.Claims.ToList();

        foreach (var claim in claims)
        {
            Log.Information("New Claim: {claimId}", claim.Id);
        }
        return claims[0].Id;
    }
}
