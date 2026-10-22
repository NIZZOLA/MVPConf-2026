using System.ComponentModel;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;

namespace ConsoleAgentFramework;

public static class FunctionToolsSamples
{
    [Description("Obter o clima para um local fornecido.")]
    public static string GetWeather([Description("O local para obter o clima.")] string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            return "Informe um local válido para consultar o clima.";

        try
        {
            using var client = new HttpClient();

            var encodedLocation = Uri.EscapeDataString(location);
            var geoUrl = $"https://geocoding-api.open-meteo.com/v1/search?name={encodedLocation}&count=1&language=pt&format=json";
            var geoResponse = client.GetStringAsync(geoUrl).GetAwaiter().GetResult();

            using var geoDocument = JsonDocument.Parse(geoResponse);
            if (!geoDocument.RootElement.TryGetProperty("results", out var results) ||
                results.ValueKind != JsonValueKind.Array ||
                results.GetArrayLength() == 0)
            {
                return $"Não foi possível localizar '{location}' para consultar o clima.";
            }

            var first = results[0];
            var name = first.TryGetProperty("name", out var nameProperty) ? nameProperty.GetString() ?? location : location;
            var country = first.TryGetProperty("country", out var countryProperty) ? countryProperty.GetString() : null;
            var latitude = first.GetProperty("latitude").GetDouble();
            var longitude = first.GetProperty("longitude").GetDouble();
            var timezone = first.TryGetProperty("timezone", out var timezoneProperty) ? timezoneProperty.GetString() ?? "auto" : "auto";

            var latitudeText = latitude.ToString(CultureInfo.InvariantCulture);
            var longitudeText = longitude.ToString(CultureInfo.InvariantCulture);
            var weatherUrl = $"https://api.open-meteo.com/v1/forecast?latitude={latitudeText}&longitude={longitudeText}&current=temperature_2m,weather_code&timezone={Uri.EscapeDataString(timezone)}";

            var weatherResponse = client.GetStringAsync(weatherUrl).GetAwaiter().GetResult();
            using var weatherDocument = JsonDocument.Parse(weatherResponse);

            if (!weatherDocument.RootElement.TryGetProperty("current", out var current) ||
                !current.TryGetProperty("temperature_2m", out var temperatureElement) ||
                !current.TryGetProperty("weather_code", out var weatherCodeElement))
            {
                return $"Não foi possível obter o clima atual para '{location}'.";
            }

            var temperature = temperatureElement.GetDouble();
            var weatherCode = weatherCodeElement.GetInt32();
            var description = GetWeatherDescription(weatherCode);
            var cityLabel = string.IsNullOrWhiteSpace(country) ? name : $"{name}, {country}";

            return $"O clima em {cityLabel} está {description.ToLowerInvariant()} com temperatura atual de {temperature.ToString("0.#", CultureInfo.InvariantCulture)}°C.";
        }
        catch (Exception ex)
        {
            return $"Não foi possível consultar o clima para '{location}'. Detalhe: {ex.Message}";
        }
    }

    private static string GetWeatherDescription(int weatherCode)
    {
        return weatherCode switch
        {
            0 => "céu limpo",
            1 or 2 or 3 => "parcialmente nublado",
            45 or 48 => "névoa",
            51 or 53 or 55 or 56 or 57 => "garoa",
            61 or 63 or 65 or 66 or 67 => "chuva",
            71 or 73 or 75 or 77 or 79 => "neve",
            80 or 81 or 82 => "pancadas de chuva",
            85 or 86 => "pancadas de neve",
            95 or 96 or 99 => "trovoada",
            _ => "condições climáticas variáveis"
        };
    }
}
