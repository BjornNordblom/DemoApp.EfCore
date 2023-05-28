public sealed class FindClaimByIdQueryHandler : IQueryHandler<FindClaimByIdQuery, ClaimDto>
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public FindClaimByIdQueryHandler(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async ValueTask<ClaimDto> Handle(
        FindClaimByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var requestClaimId = _mapper.StringToClaimId(request.Id);
        var claim = await _context.Claims
            .Include(c => c.Creditor)
            .Include(c => c.ClaimDebtors)
            .ThenInclude(cd => cd.Debtor)
            .Where(c => c.Id == requestClaimId)
            .FirstOrDefaultAsync(cancellationToken);
        return _mapper.ToClaimDto(claim);
    }
}
