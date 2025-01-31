# Currency Converter Application Tutorial

This tutorial will guide you through the process of building a simple Currency Converter application using C# and SplashKit. The application will fetch currency conversion rates from an external API and display the conversion result on a graphical user interface.

# Table of Contents

1. [Prerequisites](#prerequisites)
   - C# and .NET SDK
   - SplashKit SDK

2. [Setup Instructions](#setup-instructions)
   - Install Required Tools
   - Clone the Repository

3. [Project Structure](#project-structure)
   - Overview of Project Structure

4. [Step 1: Install Required Tools](#step-1-install-required-tools)
   - C# and .NET SDK Installation
   - SplashKit SDK Installation

5. [Step 2: Clone the Repository](#step-2-clone-the-repository)
   - Cloning the Currency Converter Repository

6. [Step 3: Handling Asynchronous Calls](#step-3-handling-asynchronous-calls)
   - Using `async` and `await` in C#
   - Asynchronous Method to Handle Conversion Logic
   - Code Implementation for Conversion Logic

7. [Step 4: Running the Program](#step-4-running-the-program)
   - Compile and Run the Program
   - Command to Run the Program

8. [Additional Resources](#additional-resources)
   - SplashKit Documentation
   - ExchangeRate API
   - C# Documentation

9. [License](#license)
   - MIT License Information

10. [Conclusion](#conclusion)
    - Final Thoughts
    - How to Extend the Application


## Prerequisites

Before you start, make sure you have the following installed:
- **C# and .NET SDK**: Download and install the .NET SDK from [Microsoft](https://dotnet.microsoft.com/download).
- **SplashKit SDK**: Install the SplashKit SDK from [SplashKit](https://www.splashkit.io/).

## Setup Instructions

### 1. Install Required Tools

To run the tutorial locally, ensure that you have the following:

- **C# and .NET SDK**: Download and install the .NET SDK from [Microsoft](https://dotnet.microsoft.com/download).
- **SplashKit SDK**: Install the SplashKit SDK from [SplashKit](https://www.splashkit.io/).
- 
## project-structure
### 2. Clone the Repository

Clone this repository to get started with the project:

```bash
git clone https://github.com/your-Anushi-Amarasinghe/currency-converter-App.git
cd currency-converter
```

## Step 3: Handling Asynchronous Calls

We use C#'s `async` and `await` to handle API requests without blocking the main application thread, ensuring smooth user interaction during API calls.

### Asynchronous Method to Handle Conversion Logic

Here is the asynchronous method that fetches the conversion rates and performs the currency conversion:

```csharp
public class CurrencyConverter
{
    private static readonly string ApiUrl = "https://v6.exchangerate-api.com/v6/{your_api_key_here}/latest/USD";

    public async Task<decimal> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency)
    {
        var rates = await FetchConversionRatesAsync();

        if (rates.ContainsKey(fromCurrency) && rates.ContainsKey(toCurrency))
        {
            decimal fromRate = rates[fromCurrency];
            decimal toRate = rates[toCurrency];

            return (amount / fromRate) * toRate;
        }

        return 0m;
    }

    private async Task<Dictionary<string, decimal>> FetchConversionRatesAsync()
    {
        using var client = new HttpClient();
        var response = await client.GetStringAsync(ApiUrl);

        var data = JsonConvert.DeserializeObject<ApiResponse>(response);
        return data.ConversionRates;
    }

    private class ApiResponse
    {
        public Dictionary<string, decimal> ConversionRates { get; set; }
    }
}
```
## Step 4: Running the Program

Once the program is set up, simply compile and run the application. You will be able to input values, select currencies, and see the results in real-time on the graphical interface.

### Command to Run the Program

To run the project, use the following command:

```bash
dotnet run
```
## Additional Resources

- [SplashKit Documentation](https://www.splashkit.io/docs/)
- [ExchangeRate API](https://www.exchangerate-api.com/)
- [C# Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/)

---

## License

This project is licensed under the MIT License - see the LICENSE file for details.

---

## Conclusion

Congratulations! You've now created a working Currency Converter application that interacts with an external API using C# and SplashKit. You can extend this tutorial to add more features, such as saving conversion history, improving the user interface, or adding more currencies.

Feel free to share this tutorial with others, and happy coding!



