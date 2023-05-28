public sealed record NegativeAmount : Amount<NegativeAmount>, IUnsignedDecimal
{
    public NegativeAmount(Amount<NegativeAmount> original)
        : base(original) { }

    public NegativeAmount(decimal value)
        : base(value)
    {
        if (value > 0)
        {
            throw new ArgumentException("Amount cannot be positive", nameof(value));
        }
    }

    public static implicit operator decimal(NegativeAmount amount) => amount.Value;

    public static implicit operator NegativeAmount(decimal value) => new(value);
}