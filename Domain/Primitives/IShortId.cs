public interface IShortId
{
    string ToShortId();
    Guid Value { get; init; }
    static abstract string Identifier { get; }
}
