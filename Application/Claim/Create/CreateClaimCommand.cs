using Mediator;

public sealed record CreateClaimCommand(
    Guid CreditorId,
    string ReferenceNo,
    List<ClaimDebtorDto> ClaimDebtors
) : IRequest<CreateClaimResponse>;
