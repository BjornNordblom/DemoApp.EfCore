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

        var debtor = new DebtorNaturalPerson
        {
            Debtor = new Debtor { Type = Debtor.DebtorType.NaturalPerson },
            FirstName = "John",
            LastName = "Doe",
            PersonalNumber = "121212-1212"
        };

        var claim = Claim.Create("ABC123", creditor.Id);
        var claim2 = Claim.Create("ABC123", creditor.Id);

        var claimDebtor = new ClaimDebtor
        {
            Debtor = debtor.Debtor,
            Claim = claim,
            Involvement = ClaimDebtor.DebtorInvolvement.Primary
        };
        var claimDebtor2 = new ClaimDebtor
        {
            Debtor = debtor.Debtor,
            Claim = claim2,
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
            claim,
            claimDebtor,
            claimDebtor2,
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

        var claim = dataContext.Claims.Include(c => c.Creditor).FirstOrDefault(x => x.Id == id);

        if (claim != null)
        {
            Log.Information(
                "Found claim: {claimId} {claimReferenceNo} {claimCreditorName}",
                claim.Id,
                claim.ReferenceNo,
                claim.Creditor.Name
            );
        }
    }

    private static ClaimId PrintContent()
    {
        Log.Warning("PrintContent");

        using var dataContext = new DataContext(_logFactory);
        var claims = dataContext.Claims.ToList();

        foreach (var claim in claims)
        {
            Log.Information("Claim: {claimId}", claim.Id);
            // Log.Information(
            //     "Claim: {claimId} {claimReferenceNo} {claimCreditorName}",
            //     claim.Id,
            //     claim.ReferenceNo,
            //     claim.Creditor.Name
            // );
            // foreach (var claimItem in claim.ClaimItems)
            // {
            //     Console.WriteLine($"ClaimItem: {claimItem.ReferenceNo}");
            //     if (claimItem.Type == ClaimItem.ClaimType.Invoice)
            //     {
            //         Console.WriteLine($"Invoice: {claimItem.Invoice.Amount}");
            //     }
            //     else
            //     {
            //         Console.WriteLine($"CreditInvoice: {claimItem.CreditInvoice.CreditedAmount}");
            //     }
            // }
        }
        return claims[0].Id;
    }
}
