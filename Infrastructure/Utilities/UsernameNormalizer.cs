using System.Text;

namespace Infrastructure.Utilities;

public static class UsernameNormalizer
{
    private static readonly Dictionary<char, char> LeetMapping = new()
    {
        { '0', 'o' },
        { '1', 'i' },
        { '2', 'z' },
        { '3', 'e' },
        { '4', 'a' },
        { '5', 's' },
        { '6', 'g' },
        { '7', 't' },
        { '8', 'b' },
        { '9', 'g' },
        { '@', 'a' },
        { '$', 's' },
        { '+', 't' },
        { '!', 'i' },
        { '(', 'c' },
        { '{', 'c' },
        { '[', 'c' },
        { ')', 'c' },
        { '}', 'c' },
        { ']', 'c' },
    };

    public static string Normalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var sb = new StringBuilder(input.Length);

        foreach (var c in input.ToLowerInvariant())
        {
            if (char.IsLetter(c))
            {
                sb.Append(c);
            }
            else if (LeetMapping.TryGetValue(c, out var mapped))
            {
                sb.Append(mapped);
            }
            // else ignore symbols like . , _ etc.
        }

        return sb.ToString();
    }
}