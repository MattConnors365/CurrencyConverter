namespace CurrencyConverter
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            /*
            Console.Write("Exchange rate from which date? Format YYYY-MM-DD");
            double rate = await Logic.GetExchangeRate("USD", "EUR", Console.ReadLine() ?? "latest");
            Console.WriteLine($"The exchange rate from USD to EUR is: {rate}");
            */

            var startDate = Logic.GetUserInput<string>("Select the start date for the range. Format YYYY-MM-DD", expectedLength: 10);
            var endDate = Logic.GetUserInput<string>("Select the end date for the range. Format YYYY-MM-DD", expectedLength: 10);

            Console.WriteLine($"The date range of {startDate} to {endDate} returned the following JSON:");
            var json = await Logic.GetExchangeRateRange("USD", "EUR", startDate, endDate);
            Console.WriteLine(json);
        }
    }
}