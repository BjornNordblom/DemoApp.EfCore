public sealed record PositiveAmount : Amount<PositiveAmount>, IUnsignedDecimal
{
    public PositiveAmount(Amount<PositiveAmount> original)
        : base(original) { }

    public PositiveAmount(decimal value)
        : base(value)
    {
        if (value < 0)
        {
            throw new ArgumentException("Amount cannot be negative", nameof(value));
        }
    }

    public static implicit operator decimal(PositiveAmount amount) => amount.Value;

    public static implicit operator PositiveAmount(decimal value) => new(value);
}
