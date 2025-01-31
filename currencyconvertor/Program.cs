using System;
using System.Net.Http;
using System.Threading.Tasks;
using SplashKitSDK;
using Newtonsoft.Json;
using System.Collections.Generic;

public class CurrencyConverterApp
{
    private CurrencyConverter converter;
    private bool isConverting = false;

    public CurrencyConverterApp()
    {
        converter = new CurrencyConverter();
    }

    public void Run()
    {
        var window = SplashKit.OpenWindow("Currency Converter", 600, 400);

        string amountInput = "";
        string fromCurrencyInput = "USD";
        string toCurrencyInput = "EUR";
        bool isTypingAmount = false;

        while (!SplashKit.WindowCloseRequested(window))
        {
            SplashKit.ProcessEvents();
            SplashKit.ClearScreen(Color.White);

            // Draw UI with current inputs
            DrawUI(amountInput, fromCurrencyInput, toCurrencyInput);

            // Check for mouse clicks on input fields
            if (SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                if (SplashKit.PointInRectangle(SplashKit.MousePosition(), SplashKit.RectangleFrom(150, 50, 100, 30)))
                {
                    isTypingAmount = true; // Focus on amount input
                }

                // Trigger conversion only if not already converting
                if (!isConverting && SplashKit.PointInRectangle(SplashKit.MousePosition(), SplashKit.RectangleFrom(150, 200, 100, 30)))
                {
                    if (double.TryParse(amountInput, out double amount))
                    {
                        ConvertAndDisplay(amount, fromCurrencyInput, toCurrencyInput).Wait(); // Run conversion synchronously
                    }
                }

                // Change From Currency
                if (SplashKit.PointInRectangle(SplashKit.MousePosition(), SplashKit.RectangleFrom(150, 100, 100, 30)))
                {
                    fromCurrencyInput = (fromCurrencyInput == "USD") ? "AUD" : "USD";
                }

                // Change To Currency
                if (SplashKit.PointInRectangle(SplashKit.MousePosition(), SplashKit.RectangleFrom(150, 150, 100, 30)))
                {
                    toCurrencyInput = (toCurrencyInput == "EUR") ? "GBP" : "EUR";
                }
            }

            // Handle typing for the amount
            if (isTypingAmount)
            {
                foreach (var key in Enum.GetValues(typeof(KeyCode)))
                {
                    KeyCode code = (KeyCode)key;

                    // If a key is typed
                    if (SplashKit.KeyTyped(code))
                    {
                        char c = GetKeyChar(code);

                        if (c != '\0')
                        {
                            amountInput += c; // Append valid characters
                        }
                        else if (code == KeyCode.BackspaceKey && amountInput.Length > 0)
                        {
                            amountInput = amountInput.Substring(0, amountInput.Length - 1); // Remove last character
                        }
                        else if (code == KeyCode.ReturnKey)
                        {
                            isTypingAmount = false; // Exit typing mode
                        }
                    }
                }
            }

            SplashKit.RefreshScreen();
        }
    }

    private async Task ConvertAndDisplay(double amount, string fromCurrency, string toCurrency)
    {
        isConverting = true;  // Lock out new conversions
        double result = await converter.ConvertCurrency(amount, fromCurrency, toCurrency);

        // Show conversion result on the UI
        SplashKit.ClearScreen(Color.White);  // Clear the screen to avoid overlap with previous UI
        SplashKit.DrawText($"Amount: {amount} {fromCurrency}", Color.Black, 20, 50);
        SplashKit.DrawText($"Converted: {result} {toCurrency}", Color.Black, 20, 100);
        SplashKit.RefreshScreen();  // Ensure the result gets displayed

        await Task.Delay(2000);  // Keep the result on screen for a brief time

        // Clear the screen and reset state
        SplashKit.ClearScreen(Color.White);
        isConverting = false;  // Allow new conversions
        SplashKit.RefreshScreen();  // Allow UI to show the cleared screen
    }

    private void DrawUI(string amountInput, string fromCurrency, string toCurrency)
    {
        // Draw UI labels
        SplashKit.DrawText("Amount:", Color.Black, 20, 50);
        SplashKit.DrawText("From Currency:", Color.Black, 20, 100);
        SplashKit.DrawText("To Currency:", Color.Black, 20, 150);
        SplashKit.DrawText("Convert", Color.Black, 20, 200);

        // Draw UI fields
        SplashKit.DrawRectangle(Color.Black, 150, 50, 100, 30);
        SplashKit.DrawRectangle(Color.Black, 150, 100, 100, 30);
        SplashKit.DrawRectangle(Color.Black, 150, 150, 100, 30);
        SplashKit.DrawRectangle(Color.Black, 150, 200, 100, 30);

        // Display current inputs
        SplashKit.DrawText(amountInput, Color.Black, 160, 60);
        SplashKit.DrawText(fromCurrency, Color.Black, 160, 110);
        SplashKit.DrawText(toCurrency, Color.Black, 160, 160);
    }

    private char GetKeyChar(KeyCode key)
    {
        // Handle numeric keys (0-9 on the keyboard)
        if (key >= KeyCode.Num0Key && key <= KeyCode.Num9Key)
        {
            return (char)(key - KeyCode.Num0Key + '0'); // Convert KeyCode to '0'-'9'
        }

        // Handle alphabet keys (A-Z)
        if (key >= KeyCode.AKey && key <= KeyCode.ZKey)
        {
            return (char)(key - KeyCode.AKey + 'A'); // Convert KeyCode to 'A'-'Z'
        }

        // Handle period key for decimal values
        if (key == KeyCode.PeriodKey)
        {
            return '.'; // Return decimal point
        }

        return '\0'; // Return null character for unsupported keys
    }
}

public class CurrencyConverter
{
    private static readonly HttpClient client = new HttpClient();

    public async Task<double> GetExchangeRate(string fromCurrency, string toCurrency)
{
    string apiKey = "e91420e5b99d8243d2be09a9"; // Replace with your actual API key
    string url = $"https://v6.exchangerate-api.com/v6/e91420e5b99d8243d2be09a9/latest/{fromCurrency}";

    try
    {
        var response = await client.GetStringAsync(url);
        Console.WriteLine($"API Response: {response}");  // Log the response to check the content

        var data = JsonConvert.DeserializeObject<ExchangeRateResponse>(response);

        if (data?.ConversionRates != null && data.ConversionRates.ContainsKey(toCurrency))
        {
            double rate = data.ConversionRates[toCurrency];
            Console.WriteLine($"Conversion Rate from {fromCurrency} to {toCurrency}: {rate}");  // Log the conversion rate
            return rate;
        }

        // Handle case where rate is not found in response
        Console.WriteLine($"Error: Conversion rate for {toCurrency} not found in response.");
        return 0.0; // Return 0 if the rate is not found
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error occurred while fetching conversion rate: {ex.Message}");
        return 0.0;
    }
}

    public async Task<double> ConvertCurrency(double amount, string fromCurrency, string toCurrency)
    {
        double rate = await GetExchangeRate(fromCurrency, toCurrency);
        if (rate == 0)
        {
            Console.WriteLine($"Unable to convert {amount} from {fromCurrency} to {toCurrency}. Returning 0.");
            return 0;
        }
        double result = amount * rate;
        Console.WriteLine($"Converted amount: {amount} {fromCurrency} = {result} {toCurrency}");
        return result;
    }
}


public class ExchangeRateResponse
{
    [JsonProperty("conversion_rates")]
    public Dictionary<string, double> ConversionRates { get; set; } = new Dictionary<string, double>();
}

public class Program
{
    public static void Main(string[] args)
    {
        CurrencyConverterApp app = new CurrencyConverterApp();
        app.Run();
    }
}
