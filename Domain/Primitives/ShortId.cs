using Microsoft.AspNetCore.WebUtilities;

public abstract class ShortId : IShortId
{
    public static string Separator => ":";
    public static string Identifier => throw new NotImplementedException();
    public Guid Value { get; init; }

    public ShortId()
    {
        this.Value = System.Guid.NewGuid();
    }

    public ShortId(Guid value)
    {
        this.Value = value;
    }

    // public static T Create(string value)
    // {
    //     return Parse(value);
    // }

    public static implicit operator Guid(ShortId shortGuid) => shortGuid.Value;

    // public static implicit operator String(ShortId<T> shortId)
    // {
    //     return $"{T.Identifier}{Separator}{shortId.ToShortId()}";
    // }

    // public override string ToString()
    // {
    //     return $"{T.Identifier}{Separator}{ToShortId()}";
    // }

    // public string ToShortId()
    // {
    //     Span<byte> bytes = stackalloc byte[16];
    //     Value.TryWriteBytes(bytes);
    //     return WebEncoders.Base64UrlEncode(bytes);
    // }

    // public static string ToShortId(string input)
    // {
    //     var indexOfSplit = input.IndexOf(Separator);
    //     var strGuid = input.Substring(indexOfSplit + 1);
    //     var parsedGuid = new Guid(WebEncoders.Base64UrlDecode(strGuid));
    //     return ToShortId(parsedGuid);
    // }

    // public static string ToShortId(Guid input)
    // {
    //     Span<byte> bytes = stackalloc byte[16];
    //     input.TryWriteBytes(bytes);
    //     return WebEncoders.Base64UrlEncode(bytes);
    // }

    public static IShortId? Parse(string input)
    {
        var indexOfSplit = input.IndexOf(Separator);
        if (indexOfSplit < 0)
        {
            throw new ArgumentException($"Invalid {nameof(input)}, no separator: {input}");
        }
        var prefix = input.Substring(0, indexOfSplit);
        ShortIdIdentifiers.All.TryGetValue(prefix, out var idType);
        if (idType is null)
        {
            throw new ArgumentException($"Invalid {prefix}, no type info: {input}");
        }
        var result = (IShortId?)Activator.CreateInstance(idType, input);

        return result;
    }

    public static bool TryParse(string input, out IShortId? result)
    {
        if (input is null)
        {
            result = default;
            return false;
        }
        var indexOfSplit = input.IndexOf(':');
        if (indexOfSplit < 0)
        {
            result = default;
            return false;
        }
        var prefix = input.Substring(0, indexOfSplit);
        try
        {
            result = Parse(input);
            var p = result.GetType().GetProperty("Identifier")?.GetValue(result);
            if (p != prefix)
            {
                result = default;
                return false;
            }

            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }
}
