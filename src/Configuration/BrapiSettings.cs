namespace stock_quote_alert.src.Configuration;

public class BrapiSettings
{
    public string BaseUrl {get;set;} = string.Empty;
    public string ApiKey {get; set;} = string.Empty;
    public string? Delay {get; set;}
    
}