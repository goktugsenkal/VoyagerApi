using Infrastructure.Utilities;

namespace Infrastructure.Services;

using Microsoft.Extensions.Caching.Memory;
using System.IO;

public class BadWordsService
{
    private readonly string _badWordsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Utilities", "bad-words.txt");
    
    private readonly MemoryCache _cache = new(new MemoryCacheOptions
    {
        SizeLimit = 1000 // cache max 1000 bad words temporarily
    });

    // cache max 1000 bad words temporarily

    public bool ContainsBadWord(string username)
    {
        var normalizedUsername = UsernameNormalizer.Normalize(username);

        Console.WriteLine("executing ContainsBadWord for: " + username);

        foreach (var line in File.ReadLines(_badWordsFilePath))
        {
            var word = line.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(word)) continue;

            if (!_cache.TryGetValue(word, out bool _))
            {
                _cache.Set(word, true, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30),
                    Size = 1
                });
            }

            if (normalizedUsername.Contains(word, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Username contains bad word: " + username);
                return true;
            }
        }
        
        Console.WriteLine("Username doesnt contain a bad word.");
        return false;
    }
}
