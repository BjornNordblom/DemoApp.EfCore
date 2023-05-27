using Microsoft.AspNetCore.WebUtilities;
using Riok.Mapperly;
using Riok.Mapperly.Abstractions;

public record ClaimDto
{
    public string Id { get; init; } = default!;
    public string ReferenceNo { get; init; } = default!;

    public List<ClaimDebtorDto> Debtors { get; init; } = new();
}

public record ClaimDebtorDto
{
    public DebtorDto Debtor { get; init; } = null!;
    public string Involvement { get; init; } = string.Empty;
}

public record DebtorDto
{
    public DebtorId Id { get; init; }
    public string Type { get; init; } = string.Empty;
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

[Mapper]
public partial class ClaimMapper
{
    public partial ClaimDto ToClaimDto(Claim claim);

    //public partial ClaimDebtorResponseDto ToClaimDebtorResponseDto(ClaimDebtor claimDebtor);
    public partial ClaimDebtorResponseDto ToClaimDebtorResponse(ClaimDebtor claimDebtor);

    public partial ClaimDebtorRequest ToClaimDebtorRequest(ClaimDebtorRequestDto claimDebtor);

    public partial ClaimDebtor ToClaimDebtor(ClaimDebtorResponseDto claimDebtorDto);

    private string ClaimIdToString(ClaimId claimId) => GuidToShortId("claim", claimId.Value);

    private string DebtorIdToString(DebtorId debtorId) => GuidToShortId("debtor", debtorId.Value);

    private ClaimId StringToClaimId(String shortId) => new ClaimId(ShortIdToGuid("claim", shortId));

    private DebtorId StringToDebtorId(String shortId) =>
        new DebtorId(ShortIdToGuid("debtor", shortId));

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
}
