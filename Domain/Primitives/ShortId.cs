using Microsoft.AspNetCore.WebUtilities;

public abstract class ShortId<T>
    where T : notnull, IShortId, new()
{
    public static string Separator => ":";

    public Guid Value { get; init; }

    public ShortId()
    {
        this.Value = System.Guid.NewGuid();
    }

    public ShortId(Guid value)
    {
        this.Value = value;
    }

    public static T Create(string value)
    {
        return Parse(value);
    }

    public static implicit operator Guid(ShortId<T> shortGuid) => shortGuid.Value;

    public static implicit operator String(ShortId<T> shortId)
    {
        return $"{T.Identifier}{Separator}{shortId.ToShortId()}";
    }

    public override string ToString()
    {
        return $"{T.Identifier}{Separator}{ToShortId()}";
    }

    public string ToShortId()
    {
        Span<byte> bytes = stackalloc byte[16];
        Value.TryWriteBytes(bytes);
        return WebEncoders.Base64UrlEncode(bytes);
    }

    public static string ToShortId(string input)
    {
        var indexOfSplit = input.IndexOf(Separator);
        var strGuid = input.Substring(indexOfSplit + 1);
        var parsedGuid = new Guid(WebEncoders.Base64UrlDecode(strGuid));
        return ToShortId(parsedGuid);
    }

    public static string ToShortId(Guid input)
    {
        Span<byte> bytes = stackalloc byte[16];
        input.TryWriteBytes(bytes);
        return WebEncoders.Base64UrlEncode(bytes);
    }

    public static T Parse(string input)
    {
        var indexOfSplit = input.IndexOf(Separator);
        if (indexOfSplit < 0)
        {
            throw new ArgumentException(
                $"Invalid {typeof(T)}, no type info, expected prefix {T.Identifier}: {input}"
            );
        }
        var prefix = input.Substring(0, indexOfSplit);
        if (prefix != T.Identifier)
        {
            throw new ArgumentException(
                $"Invalid {typeof(T)}, expected prefix {T.Identifier}: {input}"
            );
        }
        input = input.Substring(indexOfSplit + 1);
        var parsedGuid = new Guid(WebEncoders.Base64UrlDecode(input.ToString()));
        var result = new T() { Value = parsedGuid };
        return result;
    }

    public static bool TryParse(string input, out T? result)
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
        if (prefix != T.Identifier)
        {
            result = default;
            return false;
        }
        try
        {
            result = Parse(input);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }
}
