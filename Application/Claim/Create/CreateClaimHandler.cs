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
        var createdClaim = await _context.Claims
            .Include(c => c.Creditor)
            .Include(c => c.ClaimDebtors)
            .ThenInclude(cd => cd.Debtor)
            .FirstOrDefaultAsync(x => x.ClaimId == claim.ClaimId, cancellationToken);
        if (createdClaim == null)
        {
            throw new Exception("Claim not found");
        }
        return _mapper.ToClaimDto(createdClaim);
    }
}
