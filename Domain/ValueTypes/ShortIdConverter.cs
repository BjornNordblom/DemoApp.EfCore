using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class ShortIdConverter<T> : ValueConverter<T, Guid>
    where T : IShortId
{
    public ShortIdConverter()
        : base(id => id.Value, value => (T)Activator.CreateInstance(typeof(T), value)) { }
}
