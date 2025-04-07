using System.Text.RegularExpressions;
using Core.Dtos;
using Core.Interfaces;
using Core.Interfaces.Repositories;

namespace Infrastructure.Services;

public class SearchService(IS3Service s3Service, ISearchRepository searchRepository) : ISearchService
{
    public async Task<List<SearchResultDto>> GlobalSearchAsync(string query)
    {
        var voyagesByContent = await searchRepository.SearchVoyagesAsync(query);
        var voyagesByStop = await searchRepository.SearchVoyagesByStopMatchAsync(query);
        var users = await searchRepository.SearchUsersAsync(query);

        var allVoyages = voyagesByContent
            .Concat(voyagesByStop)
            .DistinctBy(v => v.Id)
            .Select(v => new SearchResultDto
            {
                Type = "voyage",
                Id = v.Id,
                Title = v.Title,
                Subtitle = v.LocationName,
                ImageUrl = s3Service.GeneratePreSignedDownloadUrl(v.ThumbnailUrl, TimeSpan.FromMinutes(15)),
                Snippet = GetSnippet(v.Description, query) ?? GetSnippet(v.Title, query) ?? GetSnippet(v.LocationName, query),
                MatchScore = ScoreText(v.Title, query) + ScoreText(v.Description, query) + ScoreText(v.LocationName, query)
            });

        var userResults = users.Select(u => new SearchResultDto
        {
            Type = "user",
            Id = u.Id,
            Title = u.Username,
            Subtitle = u.Bio,
            ImageUrl = s3Service.GeneratePreSignedDownloadUrl(u.ProfilePictureUrl, TimeSpan.FromMinutes(15)),
            MatchScore = ScoreText(u.Username, query) + ScoreText(u.Bio, query) + ScoreText(u.FirstName + " " + u.LastName, query)
        });

        return allVoyages.Concat(userResults)
            .OrderByDescending(x => x.MatchScore)
            .ToList();
    }
    
    /// <summary>
    /// Extracts a snippet around the first word that matches the query.
    /// It includes up to <paramref name="radius"/> words before and after the match.
    /// </summary>
    /// <param name="text">The full text to search in (e.g., title, description, bio).</param>
    /// <param name="query">The search query to match.</param>
    /// <param name="radius">How many words before and after to include (default: 3).</param>
    /// <returns>A short snippet string, or null if no match is found.</returns>
    private static string? GetSnippet(string text, string query, int radius = 3)
    {
        if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(query))
            return null;

        // Split the text into individual words using spaces
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // Loop through each word to find a match with the query (case-insensitive)
        for (var i = 0; i < words.Length; i++)
        {
            if (words[i].Contains(query, StringComparison.OrdinalIgnoreCase))
            {
                // Calculate the bounds of the snippet
                var start = Math.Max(0, i - radius);
                var end = Math.Min(words.Length - 1, i + radius);

                // Join the words in the range and return them as a single snippet string
                return string.Join(" ", words[start..(end + 1)]) + "...";
            }
        }

        // If no match is found, return null
        return null;
    }


    /// <summary>
    /// Calculates a match score for how relevant the given text is to the search query.
    /// Higher scores indicate stronger matches.
    /// </summary>
    /// <param name="text">The full text to evaluate.</param>
    /// <param name="query">The query to look for inside the text.</param>
    /// <returns>A numeric score indicating relevance.</returns>
    private static double ScoreText(string? text, string query)
    {
        if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(query))
            return 0;

        var score = 0;

        // highest weight: exact full match
        if (text.Equals(query, StringComparison.OrdinalIgnoreCase))
            score += 100;

        // good weight: query is at the start of the text
        else if (text.StartsWith(query, StringComparison.OrdinalIgnoreCase))
            score += 50;

        // medium weight: query appears somewhere in the text
        else if (text.Contains(query, StringComparison.OrdinalIgnoreCase))
            score += 25;

        // add a small bonus for each appearance of the query in the text
        var frequency = Regex.Matches(text, Regex.Escape(query), RegexOptions.IgnoreCase).Count;
        score += frequency * 5;

        return score;
    }
}