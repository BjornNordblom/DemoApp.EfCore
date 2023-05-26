using System.Reflection;

public interface IShortIdFactory
{
    bool Validate(string shortIdValue);
    IShortId? CreateShortId(string shortIdValue);
    bool TryParse(string shortIdValue, out IShortId? shortId);
}

public class ShortIdFactory : IShortIdFactory
{
    public static string Separator => ":";

    public IShortId? CreateShortId(string shortIdValue)
    {
        var prefix = shortIdValue.Substring(0, shortIdValue.IndexOf(Separator));

        ShortIdIdentifiers.All.TryGetValue(prefix, out var idType);
        if (idType is null)
            return default;
        // var newId = (IShortId?)
        //     idType.InvokeMember(
        //         "Create",
        //         BindingFlags.InvokeMethod,
        //         null,
        //         null,
        //         new object[] { shortIdValue }
        //     );

        return (IShortId?)Activator.CreateInstance(idType, shortIdValue);
    }

    public bool Validate(string input)
    {
        TryParse(input, out var result);
        return (result is null);
    }

    public bool TryParse(string input, out IShortId? result)
    {
        try
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
            var idType = ShortIdIdentifiers.All[prefix];
            if (idType is null)
            {
                result = default;
                return false;
            }

            result = CreateShortId(input);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }
}
