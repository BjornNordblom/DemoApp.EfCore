public static class ShortIdIdentifiers
{
    public static Dictionary<string, Type> All =>
        new() { { "claim", typeof(ClaimId) }, { "debtor", typeof(DebtorId) } };
}
