public sealed class CreateClaimHandler : IRequestHandler<CreateClaimCommand, CreateClaimResponse>
{
    private readonly DbContext _context;
    private readonly IMapper _mapper;

    public CreateClaimHandler(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async ValueTask<CreateClaimResponse> Handle(
        CreateClaimCommand command,
        CancellationToken cancellationToken
    )
    {
        var claim = _mapper.ToClaim(command);
        await _context.AddAsync(claim);
        await _context.SaveChangesAsync(cancellationToken);
        var strClaimId = _mapper.ClaimIdToString(claim.Id) ?? string.Empty;
        return new CreateClaimResponse(strClaimId, claim.ReferenceNo);
        // var claim = _mapper.Map<Claim>(command);
        // await _claimRepository.AddAsync(claim);
        // return _mapper.Map<CreateClaimResponse>(claim);
    }
}
