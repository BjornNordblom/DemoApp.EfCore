using Microsoft.EntityFrameworkCore;
using Serilog;

public static class Program
{
    public static async Task Main()
    {
        Log.Logger = new LoggerConfiguration().Enrich
            .FromLogContext()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
            )
            .CreateLogger();
        ;

        var _logFactory = new LoggerFactory().AddSerilog(Log.Logger);
        try
        {
            await using var dataContext = new DataContext(_logFactory);
            await dataContext.Database.EnsureDeletedAsync();
            await dataContext.Database.EnsureCreatedAsync();

            // var creditor = new Creditor { Name = "ABC Company", ParentCreditorId = 0 };
            // await dataContext.AddAsync(creditor);
            // await dataContext.SaveChangesAsync();

            var creditor = new Creditor { Id = Guid.NewGuid(), Name = "ABC Company" };

            var debtor = new DebtorNaturalPerson
            {
                Debtor = new Debtor { Type = Debtor.DebtorType.NaturalPerson },
                FirstName = "John",
                LastName = "Doe",
                PersonalNumber = "121212-1212"
            };

            var claim = Claim.Create("ABC123", creditor.Id);

            // {
            //     Creditor = new Creditor { Name = "ABC Company" },
            //     ReferenceNo = "ABC123"
            // };

            var claimDebtor = new ClaimDebtor
            {
                Debtor = debtor.Debtor,
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
                claimInvoice,
                claimCreditInvoice
            );
            await dataContext.SaveChangesAsync();

            PrintContent(dataContext);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void PrintContent(DataContext dataContext)
    {
        var claims = dataContext.Claims.ToList();

        foreach (var claim in claims)
        {
            Log.Information($"Claim: {claim.Id} {claim.ReferenceNo} {claim.Creditor.Name}");
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
    }
}
