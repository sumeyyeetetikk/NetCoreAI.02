using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {

        Console.Write("Text to Image: ");
        string prompt = Console.ReadLine();

        string token = "";
        string apiUrl = "https://api.replicate.com/v1/predictions";

        var requestBody = new
        {
            version = "7762fd07cf82c948538e41f63f77d685e02b063e37e496e96eefd46c929f9bdc",
            input = new
            {
                prompt = prompt
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        using var client = new HttpClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", token);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        Console.WriteLine("Image Createing...");

        var response = await client.PostAsync(apiUrl, content);
        string responseContent = await response.Content.ReadAsStringAsync();

        Console.WriteLine("Api Yanıtı: ");
        Console.WriteLine(responseContent);
    }
}












































