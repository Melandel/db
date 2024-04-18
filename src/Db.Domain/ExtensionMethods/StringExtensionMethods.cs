namespace Db.Domain.ExtensionMethods;

public static class StringExtensionMethods
{
    public static string TrimQuotes(this string str)
    => str switch
    {
        null => null,
        _ => str.Trim(new[] { '"', '\'' })
    };
}
