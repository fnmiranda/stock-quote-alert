using stock_quote_alert.src.Models;
using stock_quote_alert.src.Services.Stock;

namespace StockQuoteAlert.Tests;

public class StockEvaluatorTests
{
    private readonly StockService _service = new();

    #region EvaluateQuote - Primeiro Alerta (lastSendPrice <= 0)

    [Theory]
    [InlineData(36.0, AlertType.SELL)] // Preço acima do máximo -> Vender
    [InlineData(29.0, AlertType.BUY)]  // Preço abaixo do mínimo -> Comprar
    [InlineData(32.0, AlertType.HOLD)] // Preço entre os limites -> Manter
    public void EvaluateQuote_WhenFirstRun_ShouldEvaluateWithoutSensibility(decimal currentPrice, AlertType expectedAlert)
    {
        // Arrange
        var quote = new Quote { Symbol = "", ShortName = "", RegularMarketPrice = currentPrice, RegularMarketChangePercent = 0m, Currency = "" };
        decimal minPrice = 30.0m;
        decimal maxPrice = 35.0m;
        decimal lastSendPrice = 0m; 

        // Act
        var result = _service.EvaluateQuote(quote, lastSendPrice, minPrice, maxPrice);

        // Assert
        Assert.Equal(expectedAlert, result);
    }

    #endregion

    #region EvaluateQuote - Com Alertas Anteriores (lastSendPrice > 0, Sensibilidade = 3%)

    [Theory]
    // Preço > maxPrice e Variação > 3% -> Vender (de 35.0 para 36.10 = ~3.14% de variação)
    [InlineData(36.10, 35.0, AlertType.SELL)]
    
    // Preço > maxPrice mas Variação <= 3% -> Hold (de 35.0 para 35.50 = ~1.43% de variação)
    [InlineData(35.50, 35.0, AlertType.HOLD)]
    
    // Preço < minPrice e Variação > 3% -> Comprar (de 30.0 para 29.00 = ~3.33% de variação)
    [InlineData(29.00, 30.0, AlertType.BUY)]
    
    // Preço < minPrice mas Variação <= 3% -> Hold (de 30.0 para 29.80 = ~0.66% de variação)
    [InlineData(29.80, 30.0, AlertType.HOLD)]
    
    // Preço entre os limites -> Hold
    [InlineData(32.00, 30.0, AlertType.HOLD)]
    public void EvaluateQuote_WhenSubsequentRun_ShouldRespectSensibilityThreshold(
        decimal currentPrice, 
        decimal lastSendPrice, 
        AlertType expectedAlert)
    {
        // Arrange
        var quote = new Quote { Symbol = "", ShortName = "", RegularMarketPrice = currentPrice, RegularMarketChangePercent = 0m, Currency = "" };
        decimal minPrice = 30.0m;
        decimal maxPrice = 35.0m;

        // Act
        var result = _service.EvaluateQuote(quote, lastSendPrice, minPrice, maxPrice);

        // Assert
        Assert.Equal(expectedAlert, result);
    }

    #endregion

    #region ConvertAlertType

    [Theory]
    [InlineData(AlertType.BUY, "Comprar")]
    [InlineData(AlertType.SELL, "Vender")]
    [InlineData(AlertType.HOLD, "Não variou")]
    public void ConvertAlertType_ShouldReturnCorrectString(AlertType alertType, string expectedText)
    {
        // Act
        var result = _service.ConvertAlertType(alertType);

        // Assert
        Assert.Equal(expectedText, result);
    }

    #endregion
}