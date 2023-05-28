[StronglyTypedId]
public partial struct ClaimId
{
    public static implicit operator Guid(ClaimId claimId) => claimId.Value;
}
