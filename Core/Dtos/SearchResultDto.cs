using System.ComponentModel.DataAnnotations;

namespace Core.Dtos;

public class SearchResultDto
{
    [AllowedValues("voyage", "user" )]
    public required string Type { get; set; }       
    public Guid Id { get; set; }             
    public required string Title { get; set; }
    public string? Subtitle { get; set; }    
    public string? ImageUrl { get; set; }    
    public string? Snippet { get; set; }
    public double MatchScore { get; set; }
}
