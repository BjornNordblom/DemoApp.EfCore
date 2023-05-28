[StronglyTypedId]
public partial struct CreditorId
{
    public static implicit operator Guid(CreditorId creditorId) => creditorId.Value;
}
