using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Api.LeagueOfLegends;

[ApiController]
[Route("lol")]
public class LolController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly string _riotApiKey;

    public LolController(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        _httpClient = httpClientFactory.CreateClient();
        _riotApiKey = config["Riot:ApiKey"]
                      ?? throw new ArgumentNullException("Riot:ApiKey not configured");
        _httpClient.DefaultRequestHeaders.Add("X-Riot-Token", _riotApiKey);
    }

    // Fetch summoner data by name
    [HttpGet("recent-match")]
    public async Task<IActionResult> GetRecentMatchDetails()
    {
        // 1) Get the latest match ID
        var listUrl = $"https://europe.api.riotgames.com/lol/match/v5/matches/by-puuid/6ADtgna60Oct_uddRozNzWdLFlFeBiAJvAXJPAMKEM61BA9NehRGKP19MEctJY5KcmCsCZ_dkWkkPw/ids?start=0&count=1";
        var listResp = await _httpClient.GetAsync(listUrl);
        if (!listResp.IsSuccessStatusCode)
            return StatusCode((int)listResp.StatusCode, await listResp.Content.ReadAsStringAsync());

        var idJson = await listResp.Content.ReadAsStringAsync();
        var ids = JsonSerializer.Deserialize<string[]>(idJson);
        if (ids == null || ids.Length == 0)
            return NotFound("No matches found for this PUUID.");

        var matchId = ids[0];

        // 2) Fetch match details
        var matchUrl = $"https://europe.api.riotgames.com/lol/match/v5/matches/{matchId}";
        var matchResp = await _httpClient.GetAsync(matchUrl);
        if (!matchResp.IsSuccessStatusCode)
            return StatusCode((int)matchResp.StatusCode, await matchResp.Content.ReadAsStringAsync());

        var matchContent = await matchResp.Content.ReadAsStringAsync();
        return Content(matchContent, "application/json");
    }

    // Fetch match details by match ID
    [HttpGet("match/{region}/{matchId}")]
    public async Task<IActionResult> GetMatchById(string region, string matchId)
    {
        if (string.IsNullOrWhiteSpace(region) || string.IsNullOrWhiteSpace(matchId))
            return BadRequest("Region and matchId are required.");

        var url = $"https://{region}.api.riotgames.com/lol/match/v5/matches/{matchId}";
        var resp = await _httpClient.GetAsync(url);
        if (!resp.IsSuccessStatusCode)
            return StatusCode((int)resp.StatusCode, await resp.Content.ReadAsStringAsync());

        var content = await resp.Content.ReadAsStringAsync();
        return Content(content, "application/json");
    }
}