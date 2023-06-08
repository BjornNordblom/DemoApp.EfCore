public record Amount<T>
    where T : IUnsignedDecimal
{
    public decimal Value { get; }

    public Amount(T value)
    {
        Value = value.Value;
    }

    public static implicit operator decimal(Amount<T> amount) => amount.Value;

    public static implicit operator Amount<T>(decimal value) => new(value);

    public static T From(decimal value)
    {
        return (T)Activator.CreateInstance(typeof(T), value)!;
    }

    public static T operator +(Amount<T> left, Amount<T> right) => From(left.Value + right.Value);

    public static T operator -(Amount<T> left, Amount<T> right) => From(left.Value - right.Value);

    public static T operator *(Amount<T> left, Amount<T> right) => From(left.Value * right.Value);

    public static T operator /(Amount<T> left, Amount<T> right) => From(left.Value / right.Value);

    public static bool operator <(Amount<T> left, Amount<T> right) => left.Value < right.Value;

    public static bool operator >(Amount<T> left, Amount<T> right) => left.Value > right.Value;

    public static bool operator <=(Amount<T> left, Amount<T> right) => left.Value <= right.Value;

    public static bool operator >=(Amount<T> left, Amount<T> right) => left.Value >= right.Value;

    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
}
