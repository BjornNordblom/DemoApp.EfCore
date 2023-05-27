public interface IShortId
{
    //    string ToShortId();
    Guid Value { get; init; }
    public static abstract string Identifier { get; }
}
