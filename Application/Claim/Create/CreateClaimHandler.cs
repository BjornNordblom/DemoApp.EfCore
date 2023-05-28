public sealed class CreateClaimHandler : IRequestHandler<CreateClaimCommand, ClaimDto>
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public CreateClaimHandler(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async ValueTask<ClaimDto> Handle(
        CreateClaimCommand command,
        CancellationToken cancellationToken
    )
    {
        var claim = _mapper.ToClaim(command);
        await _context.AddAsync(claim);
        await _context.SaveChangesAsync(cancellationToken);
        var strClaimId = _mapper.ClaimIdToString(claim.Id) ?? string.Empty;
        var createdClaim = await _context.Claims
            .Include(c => c.Creditor)
            .Include(c => c.ClaimDebtors)
            .ThenInclude(cd => cd.Debtor)
            .ThenInclude(d => d.DebtorNaturalPerson)
            .FirstOrDefaultAsync(x => x.Id == claim.Id, cancellationToken);
        return _mapper.ToClaimDto(createdClaim); //new CreateClaimResponse(strClaimId, claim.ReferenceNo);
        // var claim = _mapper.Map<Claim>(command);
        // await _claimRepository.AddAsync(claim);
        // return _mapper.Map<CreateClaimResponse>(claim);
    }
}
