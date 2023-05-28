using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StronglyTypedIds;

public static class Program
{
    private static ILoggerFactory _logFactory = null!;
    private static IDataContext _dataContext = null!;
    private static IServiceProvider _serviceProvider = null!;

    private async static Task CreateDatabase()
    {
        _dataContext = new DataContext(_serviceProvider, _logFactory);
        await _dataContext.Database.EnsureDeletedAsync();
        await _dataContext.Database.EnsureCreatedAsync();
    }

    private async static Task SeedDatabaseSimple(int times)
    {
        while (times > 0)
        {
            try
            {
                var dateTime = new DateTimeService(() => DateTime.Now);
                var isAdult = new IsAdultSpecification(dateTime);
                var claim = new Claim
                {
                    ClaimId = ClaimId.New(),
                    ReferenceNo = "ABC123",
                    Creditor = new Creditor { CreditorId = CreditorId.New(), Name = "ABC Company" }
                };
                var debtor = new DebtorNaturalPerson
                {
                    DebtorId = DebtorId.New(),
                    Type = Debtor.DebtorType.NaturalPerson,
                    FirstName = "John",
                    LastName = "Doe",
                    PersonalNumber = "121212-1212",
                    DateOfBirth = dateTime.Now.AddYears(-40)
                };
                claim.AddDebtor(debtor, ClaimDebtor.DebtorInvolvement.Primary);
                // var claimDebtor = new ClaimDebtor
                // {
                //     Debtor = debtor,
                //     Claim = claim,
                //     Involvement = ClaimDebtor.DebtorInvolvement.Primary
                // };
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
                    Amount = NegativeAmount.From(-100m)
                };
                if (!isAdult.IsSatisfiedBy(debtor))
                {
                    throw new Exception("Debtor is not an adult");
                }
                await _dataContext.AddRangeAsync(debtor, claim, claimInvoice);
                await _dataContext.SaveChangesAsync();
            }
            finally
            {
                times--;
            }
        }
    }

    private async static Task SeedDatabase(DbContext dataContext)
    {
        var creditor = new Creditor { CreditorId = CreditorId.New(), Name = "ABC Company" };

        // Create a new debtor of type NaturalPerson
        var debtor = new DebtorNaturalPerson
        {
            DebtorId = DebtorId.New(),
            Type = Debtor.DebtorType.NaturalPerson,
            FirstName = "John",
            LastName = "Doe",
            PersonalNumber = "121212-1212"
        };

        var debtor2 = new DebtorNaturalPerson
        {
            DebtorId = DebtorId.New(),
            Type = Debtor.DebtorType.NaturalPerson,
            FirstName = "Jane",
            LastName = "Doe",
            PersonalNumber = "121212-1212"
        };

        var claim = Claim.Create("ABC123", creditor.CreditorId);
        var claim2 = Claim.Create("ABC123", creditor.CreditorId);

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

        var claimCreditNote = new CreditNote
        {
            ClaimItem = new ClaimItem()
            {
                Claim = claim,
                ReferenceNo = "CRED-INV123",
                Type = ClaimItem.ClaimType.CreditNote
            },
            CreditedInvoice = claimInvoice,
            Amount = NegativeAmount.From(-500m)
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
            claimCreditNote
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

        var dateTimeService = new DateTimeService();
        var userService = new UserService();
        using IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton<IDateTimeService, DateTimeService>();
                services.AddSingleton<IUserService, UserService>();
                services.AddSingleton<IClaimRepository, ClaimRepository>();
                services.AddSingleton<ConvertDomainEventsToOutboxMessagesInterceptor>();
                services.AddSingleton<UpdateAuditableInterceptor>();
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
        // Get the host serviceprovider
        _serviceProvider = host.Services;
        try
        {
            await CreateDatabase();
            //for (var i = 0; i < 10; i++)
            //{
            await SeedDatabaseSimple(6);
            //}
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
                        DebtorId = "debtor:chFxC8y6ZEqTz6TmbPYfyg",
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
            if (creditor == null)
            {
                throw new Exception("Creditor not found");
            }
            var createClaim = new CreateClaimCommand(
                mapper.CreditorIdToString(creditor.CreditorId),
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
            var resultClaimDtoId = resultClaimDto.ClaimId;
            var findClaimByIdQuery = new FindClaimByIdQuery(resultClaimDtoId);
            var findClaimResult = await mediator.Send(findClaimByIdQuery);
            Log.Information(
                System.Text.Json.JsonSerializer.Serialize(
                    findClaimResult,
                    new JsonSerializerOptions { WriteIndented = true }
                )
            );

            var getClaims = new GetClaimsQuery();
            var getClaimsResult = await mediator.Send(getClaims);
            Log.Information(
                System.Text.Json.JsonSerializer.Serialize(
                    getClaimsResult,
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
        using var dataContext = new DataContext(_serviceProvider, _logFactory);
        var debtors = dataContext.Debtors
            .Include(x => x.DebtorClaims.Where(x => x.ClaimId == id))
            .ThenInclude(x => x.Claim)
            .Where(x => x.DebtorClaims.Any(x => x.ClaimId == id))
            .ToList();
        foreach (var debtor in debtors)
        {
            Log.Information("Found debtor: {debtorId} {debtorType}", debtor.DebtorId, debtor.Type);
            foreach (var claim in debtor.DebtorClaims.Select(x => x.Claim))
            {
                if (claim is null)
                {
                    Log.Information("Claim is null");
                    continue;
                }
                Log.Information(
                    "Found claim: {claimId} {claimReferenceNo}",
                    claim.ClaimId,
                    claim.ReferenceNo
                );
            }
        }
        var claims = dataContext.Claims
            .Include(x => x.ClaimDebtors)
            .ThenInclude(x => x.Debtor)
            .Where(x => x.ClaimId == id)
            .SelectMany(x => x.ClaimDebtors)
            .ToList();
        foreach (var claim in claims)
        {
            Log.Information(
                "Found claim: {claimId} {claimReferenceNo}, {debtorId}, {debtorInvolvement}",
                claim.Claim.ClaimId,
                claim.Claim.ReferenceNo,
                claim.Debtor.DebtorId,
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

        using var dataContext = new DataContext(_serviceProvider, _logFactory);
        var claims = _dataContext.Claims.ToList();

        foreach (var claim in claims)
        {
            Log.Information("New Claim: {claimId}", claim.ClaimId);
        }
        return claims[0].ClaimId;
    }
}
