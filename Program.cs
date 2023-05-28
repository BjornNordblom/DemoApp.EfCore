using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StronglyTypedIds;

public static class Program
{
    private static ILoggerFactory _logFactory;
    private static IDataContext _dataContext;

    private static async Task CreateDatabase()
    {
        _dataContext = new DataContext(_logFactory);
        await _dataContext.Database.EnsureDeletedAsync();
        await _dataContext.Database.EnsureCreatedAsync();
    }

    private async static Task SeedDatabaseSimple()
    {
        var dateTime = new DateTimeService(() => DateTime.Now);
        var isAdult = new IsAdultSpecification(dateTime);
        var claim = new Claim
        {
            Id = ClaimId.New(),
            ReferenceNo = "ABC123",
            Creditor = new Creditor { Id = CreditorId.New(), Name = "ABC Company" }
        };
        var debtor = new DebtorNaturalPerson
        {
            Id = DebtorId.New(),
            Type = Debtor.DebtorType.NaturalPerson,
            FirstName = "John",
            LastName = "Doe",
            PersonalNumber = "121212-1212",
            DateOfBirth = dateTime.Now.AddYears(-40)
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
            Amount = PositiveAmount.From(1000m)
        };
        var claimCredit = new CreditNote
        {
            ClaimItem = new ClaimItem()
            {
                Claim = claim,
                ReferenceNo = "CRED123",
                Type = ClaimItem.ClaimType.CreditNote
            },
            Amount = NegativeAmount.From(100m)
        };
        if (!isAdult.IsSatisfiedBy(debtor))
        {
            throw new Exception("Debtor is not an adult");
        }
        await _dataContext.AddRangeAsync(debtor, claim, claimDebtor, claimInvoice);
        await _dataContext.SaveChangesAsync();
    }

    private async static Task SeedDatabase(DbContext dataContext)
    {
        var creditor = new Creditor { Id = CreditorId.New(), Name = "ABC Company" };

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
            Amount = PositiveAmount.From(1000m)
        };
        var claimInvoice2 = new Invoice
        {
            ClaimItem = new ClaimItem()
            {
                Claim = claim2,
                ReferenceNo = "INV123",
                Type = ClaimItem.ClaimType.Invoice
            },
            Amount = PositiveAmount.From(1000m)
        };

        var claimCreditInvoice = new CreditNote
        {
            ClaimItem = new ClaimItem()
            {
                Claim = claim,
                ReferenceNo = "CRED-INV123",
                Type = ClaimItem.ClaimType.CreditNote
            },
            CreditedInvoice = claimInvoice,
            Amount = new NegativeAmount(-500m)
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
        await CreateDatabase();
        var dateTimeService = new DateTimeService();
        using IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton<IDateTimeService, DateTimeService>();
                services.AddSingleton<IClaimRepository, ClaimRepository>();
                services.AddScoped<IMapper, ClaimMapper>();
                services.AddDbContext<DataContext>();
                services.AddMediator();
                services.AddLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder
                        .AddSerilog(Log.Logger)
                        .AddFilter(
                            "Microsoft.EntityFrameworkCore.Database.Command",
                            LogLevel.Information
                        )
                        .AddFilter(
                            "Microsoft.EntityFrameworkCore.Infrastructure",
                            LogLevel.Information
                        );
                });
            })
            .Build();
        try
        {
            await CreateDatabase();
            for (var i = 0; i < 10; i++)
            {
                await SeedDatabaseSimple();
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
            var mediator = host.Services.GetRequiredService<IMediator>();
            var createClaimDebtors = new List<ClaimDebtorDto>()
            {
                new ClaimDebtorDto
                {
                    Involvement = "Primary",
                    Debtor = new DebtorDto
                    {
                        Id = "debtor:chFxC8y6ZEqTz6TmbPYfyg",
                        Type = "NaturalPerson",
                        NaturalPerson = new DebtorNaturalPersonDto
                        {
                            FirstName = "Slim",
                            LastName = "Jim",
                            PersonalNumber = "000000-0000"
                        }
                    }
                }
            };
            var creditor = await _dataContext.Creditors.FirstOrDefaultAsync();
            var createClaim = new CreateClaimCommand(
                mapper.CreditorIdToString(creditor.Id),
                "XYZ098",
                createClaimDebtors
            );
            var resultClaimDto = await mediator.Send(createClaim);
            Log.Information(
                System.Text.Json.JsonSerializer.Serialize(
                    resultClaimDto,
                    new JsonSerializerOptions { WriteIndented = true }
                )
            );
            var resultClaimDtoId = resultClaimDto.Id;
            var findClaimByIdQuery = new FindClaimByIdQuery(resultClaimDtoId);
            var findClaimResult = await mediator.Send(findClaimByIdQuery);
            Log.Information(
                System.Text.Json.JsonSerializer.Serialize(
                    findClaimResult,
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
