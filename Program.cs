using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StronglyTypedIds;

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

    private async static Task SeedDatabaseSimple(DbContext dataContext)
    {
        var claim = new Claim
        {
            Id = ClaimId.New(),
            ReferenceNo = "ABC123",
            Creditor = new Creditor { Id = Guid.NewGuid(), Name = "ABC Company" }
        };
        var debtor = new DebtorNaturalPerson
        {
            Id = DebtorId.New(),
            Type = Debtor.DebtorType.NaturalPerson,
            FirstName = "John",
            LastName = "Doe",
            PersonalNumber = "121212-1212"
        };
        var claimDebtor = new ClaimDebtor
        {
            Debtor = debtor,
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
        await dataContext.AddRangeAsync(debtor, claim, claimDebtor, claimInvoice);
        await dataContext.SaveChangesAsync();
    }

    private async static Task SeedDatabase(DbContext dataContext)
    {
        var creditor = new Creditor { Id = Guid.NewGuid(), Name = "ABC Company" };

        // Create a new debtor of type NaturalPerson
        var debtor = new DebtorNaturalPerson
        {
            Id = DebtorId.New(),
            Type = Debtor.DebtorType.NaturalPerson,
            FirstName = "John",
            LastName = "Doe",
            PersonalNumber = "121212-1212"
        };

        var debtor2 = new DebtorNaturalPerson
        {
            Id = DebtorId.New(),
            Type = Debtor.DebtorType.NaturalPerson,
            FirstName = "Jane",
            LastName = "Doe",
            PersonalNumber = "121212-1212"
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
            for (var i = 0; i < 10; i++)
            {
                SeedDatabaseSimple(dataContext).Wait();
            }
            var claimId = PrintContent();
            PrintOne(new ClaimId(claimId));

            // {
            //   "ClaimId": "claim:9hUY1gjb2EWhPBRqJTd8dA",
            //   "DebtorId": "debtor:chFxC8y6ZEqTz6TmbPYfyg",
            //   "ClaimReferenceNo": "ABC123",
            //   "DebtorType": "NaturalPerson",
            //   "Involvement": "Primary"
            // }
            var claimDebtorRequestDto = new ClaimDebtorRequestDto
            {
                ClaimId = "claim:9hUY1gjb2EWhPBRqJTd8dA",
                DebtorId = "debtor:chFxC8y6ZEqTz6TmbPYfyg"
            };
            var mapper = new ClaimMapper();
            var claimDebtorRequest = mapper.ToClaimDebtorRequest(claimDebtorRequestDto);
            Log.Information(
                System.Text.Json.JsonSerializer.Serialize(
                    claimDebtorRequest,
                    new JsonSerializerOptions { WriteIndented = true }
                )
            );
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
        var mapper = new ClaimMapper();
        using var dataContext = new DataContext(_logFactory);
        var debtors = dataContext.Debtors
            .Include(x => x.DebtorClaims.Where(x => x.ClaimId == id))
            .ThenInclude(x => x.Claim)
            .Where(x => x.DebtorClaims.Any(x => x.ClaimId == id))
            .ToList();
        foreach (var debtor in debtors)
        {
            Log.Information("Found debtor: {debtorId} {debtorType}", debtor.Id, debtor.Type);
            foreach (var claim in debtor.DebtorClaims.Select(x => x.Claim))
            {
                if (claim is null)
                {
                    Log.Information("Claim is null");
                    continue;
                }
                Log.Information(
                    "Found claim: {claimId} {claimReferenceNo}",
                    claim.Id,
                    claim.ReferenceNo
                );
            }
        }
        var claims = dataContext.Claims
            .Include(x => x.ClaimDebtors)
            .ThenInclude(x => x.Debtor)
            .Where(x => x.Id == id)
            .SelectMany(x => x.ClaimDebtors)
            .ToList();
        foreach (var claim in claims)
        {
            Log.Information(
                "Found claim: {claimId} {claimReferenceNo}, {debtorId}, {debtorInvolvement}",
                claim.Claim.Id,
                claim.Claim.ReferenceNo,
                claim.Debtor.Id,
                claim.Involvement
            );
            // foreach (var debtor in claim.Debtors.Select(x => x.Debtor))
            // {
            //     Log.Information("Found debtor: {debtorId}", debtor.Id);
            // }

            // var claimDto = mapper.ToClaimDto(claim.Claim);
            Log.Information(
                System.Text.Json.JsonSerializer.Serialize(
                    mapper.ToClaimDebtorResponse(claim),
                    new JsonSerializerOptions { WriteIndented = true }
                )
            );
        }
    }

    private static Guid PrintContent()
    {
        Log.Warning("PrintContent");

        using var dataContext = new DataContext(_logFactory);
        var claims = dataContext.Claims.ToList();

        foreach (var claim in claims)
        {
            Log.Information("New Claim: {claimId}", claim.Id);
        }
        return claims[0].Id.Value;
    }
}
