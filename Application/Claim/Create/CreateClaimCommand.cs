public sealed record CreateClaimCommand(
    string CreditorId,
    string ReferenceNo,
    List<ClaimDebtorDto> ClaimDebtors
) : IRequest<ClaimDto>;
