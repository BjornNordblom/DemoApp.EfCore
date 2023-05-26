public class ClaimId : ShortId<ClaimId>, IShortId
{
    public static string Identifier => "clm";

    public ClaimId()
        : base(System.Guid.NewGuid()) { }

    public ClaimId(System.Guid guid)
        : base(guid) { }

    public ClaimId(string shortid)
        : base(Parse(shortid)) { }
}
