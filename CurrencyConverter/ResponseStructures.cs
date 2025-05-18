using System.Text.Json.Serialization;

namespace CurrencyConverter
{
    internal class ResponseStructures
    {
        public class GetRate
        {
            [JsonPropertyName("base")] public required string BaseCurrency { get; set; }
            [JsonPropertyName("date")] public required string Date { get; set; }
            [JsonPropertyName("rates")] public required Dictionary<string, double> Rate { get; set; }
        }
    }
}
