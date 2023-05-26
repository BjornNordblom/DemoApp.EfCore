public static class ShortIdIdentifiers
{
    public static Dictionary<string, Type> All =>
        new()
        {
            // { InvoiceId.Identifier, typeof(InvoiceId) },
            { ClaimId.Identifier, typeof(ClaimId) },
            // { DebtorId.Identifier, typeof(DebtorId) },
            // { PaymentAllocationId.Identifier, typeof(PaymentAllocationId) },
            // { PaymentId.Identifier, typeof(PaymentId) },
        };
}
