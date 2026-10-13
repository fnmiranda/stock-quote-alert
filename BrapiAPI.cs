using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;


public class Quote
{
    public required string Symbol { get; set; }
    public required string ShortName { get; set; }
    public required decimal RegularMarketPrice { get; set; }
    public required decimal RegularMarketChangePercent { get; set; }
    public required string Currency { get; set; }
}

public class QuoteResponse
{
    public Quote[]? Results {get;set;}
}

public class BrapiClient
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://brapi.dev/api";

    public BrapiClient(string token)
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(20)
        };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<Quote?> GetQuoteAsync(string ticker)
    {
        var url = $"{BaseUrl}/quote/{ticker}";
        var response = await _httpClient.GetStringAsync(url);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var data = JsonSerializer.Deserialize<QuoteResponse>(response, options);
        return data?.Results?[0];
    }
}    
