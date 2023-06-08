using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class AmountToUnsignedDecimalConverter<T> : ValueConverter<T, decimal>
    where T : IUnsignedDecimal
{
    public AmountToUnsignedDecimalConverter()
        : base(v => v.Value, v => (T)Activator.CreateInstance(typeof(T), v)!) { }
}
