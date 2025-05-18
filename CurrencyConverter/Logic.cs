using System;
using System.Text.Json;

namespace CurrencyConverter
{
    internal class Logic
    {
        public static dynamic GetUserInput<T>(string message, bool acceptEmpty = false, uint? expectedLength = null)
        {
            if (!message.EndsWith(" ")) { message += " "; }
            while (true)
            {
                Console.Write(message);
                string? input = Console.ReadLine();

                if (input == null)
                {
                    Console.WriteLine("Input cannot be null. Please make a valid input.");
                    continue;
                }
                if (string.IsNullOrWhiteSpace(input) && !acceptEmpty)
                {
                    Console.WriteLine("Input cannot be empty. Please make a valid input.");
                    continue;
                }
                if (expectedLength != null && input.Length != expectedLength)
                {
                    Console.WriteLine($"Input must be {expectedLength} characters long. Please make a valid input.");
                    continue;
                }
                switch (typeof(T))
                {
                    case Type t when t == typeof(string):
                        return input;
                    case Type t when t == typeof(double):
                        if (double.TryParse(input, out double result))
                        {
                            return result;
                        }
                        Console.WriteLine("Invalid input. Please enter a valid number.");
                        break;
                    default:
                        throw new NotSupportedException($"Type {typeof(T)} is not supported.");
                }
            }
        }
        public static double GetExchangedAmount(double amount, double exchangeRate)
        {
            return amount * exchangeRate;
        }
        // Skimmed the following Medium article for some insights: https://medium.com/bgl-tech/how-to-make-your-first-get-api-call-in-c-net-core-501134ee6e19
        private static readonly string baseUrl = $"https://api.frankfurter.dev/";
        private static readonly HttpClient client = new()
        {
            BaseAddress = new Uri(baseUrl)
        };
        private static void PrepareClient()
        {
            // Disable IPv6 to avoid HttpClient hanging on certain networks
            AppContext.SetSwitch("System.Net.DisableIPv6", true);
        }
        /// <summary>
        /// Gets the exchange rate between two currencies. 
        /// </summary>
        /// <param name="from">Currency to convert</param>
        /// <param name="to">Target currency</param>
        /// <param name="date">Optional. Used to get the exchange rate on a particular day. Format YYYY-MM-DD. Default = latest</param>
        /// <returns>The exchange rate between two currencies</returns>
        /// <exception cref="Exception"></exception>
        public static async Task<double> GetExchangeRate(string from, string to, string date= "latest")
        {
            PrepareClient();

            string parameters = $"v1/{date}?base={from.ToUpper()}&symbols={to.ToUpper()}";

            HttpResponseMessage response = await client.GetAsync(parameters);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Request failed: {response.StatusCode}");
            }

            string json = await response.Content.ReadAsStringAsync();

            ResponseStructures.GetRate? currencyResponse = JsonSerializer.Deserialize<ResponseStructures.GetRate>(json);

            if (currencyResponse == null || !currencyResponse.Rate.ContainsKey(to))
            {
                Console.WriteLine($"API responded with: {json}");
                throw new Exception("Currency rate not found.");
            }

            return currencyResponse.Rate[to];
        }
        public static async Task<string> GetExchangeRateRange(string from, string to, string? startDate, string? endDate)
        {
            PrepareClient();
            startDate ??= "latest";
            endDate ??= "latest";
            string parameters = $"v1/{startDate}..{endDate}?base={from.ToUpper()}&symbols={to.ToUpper()}";

            HttpResponseMessage response = await client.GetAsync(parameters);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Request failed: {response.StatusCode}");
            }

            string json = await response.Content.ReadAsStringAsync();

            return json;
        }
    }
}
