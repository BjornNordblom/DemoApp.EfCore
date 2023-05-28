public sealed record FindClaimResponse(
    string Id,
    string ReferenceNo,
    List<DebtorDto> Debtors,
    CreditorDto Creditor
);
