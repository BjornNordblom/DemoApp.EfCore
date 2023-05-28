public sealed class GetClaimsHandler : IQueryHandler<GetClaimsQuery, IEnumerable<ClaimDto>>
{
    private readonly IUserService _userService;
    private readonly IDataContext _dataContext;
    private readonly IMapper _mapper;

    public GetClaimsHandler(IUserService userService, DataContext dataContext, IMapper mapper)
    {
        _userService = userService;
        _dataContext = dataContext;
        _mapper = mapper;
    }

    public async ValueTask<IEnumerable<ClaimDto>> Handle(
        GetClaimsQuery request,
        CancellationToken cancellationToken
    )
    {
        var claims = await _dataContext.Claims
            .Include(c => c.Creditor)
            .Include(c => c.ClaimDebtors)
            .ThenInclude(cd => cd.Debtor)
            .ToListAsync(cancellationToken);

        return claims.Select(claim => _mapper.ToClaimDto(claim));
    }
}
