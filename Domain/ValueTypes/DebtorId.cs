[StronglyTypedId]
public partial struct DebtorId
{
    public static implicit operator Guid(DebtorId debtorId) => debtorId.Value;
}
