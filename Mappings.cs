[assembly: StronglyTypedIdDefaults(
    backingType: StronglyTypedIdBackingType.Guid,
    converters: StronglyTypedIdConverter.EfCoreValueConverter
        | StronglyTypedIdConverter.SystemTextJson
)]

public record ClaimDto
{
    public string Id { get; init; } = default!;
    public string ReferenceNo { get; init; } = default!;
    public CreditorDto Creditor { get; init; } = default!;
    public List<ClaimDebtorDto> ClaimDebtors { get; init; } = new();
}

public record CreditorDto
{
    public string Id { get; init; } = default!;
    public string Name { get; init; } = string.Empty;
    public string? RegistrationNumber { get; init; }
}

public record ClaimDebtorDto
{
    public DebtorDto Debtor { get; init; } = null!;
    public string Involvement { get; init; } = string.Empty;
}

public record DebtorDto
{
    public string Id { get; init; } = default!;
    public string Type { get; init; } = default!;
    public DebtorNaturalPersonDto? NaturalPerson { get; init; }
    public DebtorLegalPersonDto? LegalPerson { get; init; }
}

public record DebtorNaturalPersonDto
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? PersonalNumber { get; init; }
    public DateTime? DateOfBirth { get; init; }
}

public record DebtorLegalPersonDto
{
    public string Name { get; init; } = string.Empty;
    public string? RegistrationNumber { get; init; }
}

public record ClaimDebtorResponseDto
{
    public string ClaimId { get; init; } = default!;
    public string DebtorId { get; init; } = default!;
    public string ClaimReferenceNo { get; init; } = default!;
    public string DebtorType { get; init; } = default!;
    public string Involvement { get; init; } = default!;
}

public record ClaimDebtorRequest
{
    public ClaimId ClaimId { get; init; } = default!;
    public DebtorId DebtorId { get; init; } = default!;
}

public record ClaimDebtorRequestDto
{
    public string ClaimId { get; init; } = default!;
    public string DebtorId { get; init; } = default!;
}

public interface IMapper
{
    ClaimDto ToClaimDto(Claim claim);
    Claim ToClaim(ClaimDto claimDto);
    Claim ToClaim(CreateClaimCommand claimDto);
    ClaimDebtorResponseDto ToClaimDebtorResponse(ClaimDebtor claimDebtor);
    ClaimDebtorRequest ToClaimDebtorRequest(ClaimDebtorRequestDto claimDebtor);
    ClaimDebtor ToClaimDebtor(ClaimDebtorResponseDto claimDebtorDto);
    string GuidToShortId(String prefix, Guid id);
    Guid ShortIdToGuid(String prefix, String shortId);
    string ClaimIdToString(ClaimId claimId);
    string DebtorIdToString(DebtorId debtorId);
    ClaimId StringToClaimId(String shortId);
    DebtorId StringToDebtorId(String shortId);
    CreditorDto ToCreditorDto(Creditor creditor);
    Creditor ToCreditor(CreditorDto creditorDto);
}

[Mapper]
public partial class ClaimMapper : IMapper
{
    public partial Claim ToClaim(ClaimDto claimDto);

    public partial Claim ToClaim(CreateClaimCommand claimDto);

    public partial ClaimDto ToClaimDto(Claim claim);

    public partial ClaimDebtorResponseDto ToClaimDebtorResponse(ClaimDebtor claimDebtor);

    public partial ClaimDebtorRequest ToClaimDebtorRequest(ClaimDebtorRequestDto claimDebtor);

    public partial ClaimDebtor ToClaimDebtor(ClaimDebtorResponseDto claimDebtorDto);

    public partial CreditorDto ToCreditorDto(Creditor creditor);

    public partial Creditor ToCreditor(CreditorDto creditorDto);

    public string ClaimIdToString(ClaimId claimId) => GuidToShortId("claim", claimId.Value);

    public string DebtorIdToString(DebtorId debtorId) => GuidToShortId("debtor", debtorId.Value);

    public string CreditorIdToString(CreditorId creditorId) =>
        GuidToShortId("creditor", creditorId.Value);

    public ClaimId StringToClaimId(String shortId) => new ClaimId(ShortIdToGuid("claim", shortId));

    public DebtorId StringToDebtorId(String shortId) =>
        new DebtorId(ShortIdToGuid("debtor", shortId));

    public CreditorId StringToCreditorId(String shortId) =>
        new CreditorId(ShortIdToGuid("creditor", shortId));

    public string GuidToShortId(String prefix, Guid id)
    {
        Span<byte> bytes = stackalloc byte[16];
        id.TryWriteBytes(bytes);
        var shortGuid = WebEncoders.Base64UrlEncode(bytes);
        return $"{prefix}:{shortGuid}";
    }

    public Guid ShortIdToGuid(String prefix, String shortId)
    {
        var prefixLength = prefix.Length + 1;
        var foundPrefix = shortId.Substring(0, prefixLength);
        if (foundPrefix != $"{prefix}:")
        {
            throw new ArgumentException("Invalid shortId", nameof(shortId));
        }
        Span<byte> bytes = stackalloc byte[16];
        bytes = WebEncoders.Base64UrlDecode(shortId.Substring(prefixLength));
        //return new ClaimId(new Guid(bytes));
        return new Guid(bytes);
    }

    public decimal PositiveAmountToDecimal(PositiveAmount amount) => amount.Value;

    public PositiveAmount DecimalToPositiveAmount(decimal amount) => PositiveAmount.From(amount);
}
