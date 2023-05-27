using Microsoft.AspNetCore.WebUtilities;
using Riok.Mapperly;
using Riok.Mapperly.Abstractions;

public record ClaimDto
{
    public string Id { get; init; } = default!;
    public string ReferenceNo { get; init; } = default!;

    //public List<ClaimDebtorDto> ClaimDebtors { get; init; } = new();
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

[Mapper]
public partial class ClaimMapper
{
    [MapProperty(nameof(Claim.ClaimDebtors), nameof(ClaimDto.Debtors))]
    public partial ClaimDto ClaimToClaimDto(Claim claim);

    private string ClaimIdToString(ClaimId claimId) => ToShortId("claim", claimId.Value);

    private string DebtorIdToString(ClaimId claimId) => ToShortId("debtor", claimId.Value);

    public string ToShortId(String prefix, Guid id)
    {
        Span<byte> bytes = stackalloc byte[16];
        id.TryWriteBytes(bytes);
        var shortGuid = WebEncoders.Base64UrlEncode(bytes);
        return $"{prefix}:{shortGuid}";
    }
}
