[StronglyTypedId]
public partial struct CostId
{
    public static implicit operator Guid(CostId costId) => costId.Value;
}
