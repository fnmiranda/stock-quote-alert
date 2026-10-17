using System.Net.Http.Headers;
using System.Text.Json;

public class BrapiClient :IBrapiClient
{
    private readonly HttpClient _httpClient;
    private readonly BrapiSettings _brapiSettings;

    public BrapiClient(BrapiSettings brapiSettings)
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(20)
        };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", brapiSettings.ApiKey);
        _brapiSettings = brapiSettings;
    }

    public async Task<Quote> GetQuoteAsync(string ticker)
    {
        try
        {
            var url = $"{_brapiSettings.BaseUrl}/quote/{ticker}";
            var response = await _httpClient.GetAsync(url);

            if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new BrapiApiException($"[{DateTime.Now:HH:mm:ss}]: O ativo '{ticker}' não foi encontrado na Brapi.");
            }

            if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new BrapiApiException($"[{DateTime.Now:HH:mm:ss}]: ApiKey invalida.");
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<QuoteResponse>( responseString, options);
            return data?.Results?[0]!;
        }catch(HttpRequestException ex){
            throw new BrapiApiException ($"[{DateTime.Now:HH:mm:ss}]: Erro ao consultar API para ticket: {ticker}: {ex.Message}");
        }
    }
}    
