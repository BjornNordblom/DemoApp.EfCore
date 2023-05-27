public class ClaimRepository : IClaimRepository
{
    private readonly IDateTimeService _dateTimeService;
    private readonly IDataContext _context;
    private readonly ILogger<ClaimRepository> _logger;

    public ClaimRepository(
        IDateTimeService dateTimeService,
        IDataContext context,
        ILogger<ClaimRepository> logger
    )
    {
        _dateTimeService = dateTimeService;
        _context = context;
        _logger = logger;
    }
}
