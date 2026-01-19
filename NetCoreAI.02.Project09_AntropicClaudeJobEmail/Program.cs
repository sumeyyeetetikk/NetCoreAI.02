using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

class Program
{
    static async Task Main(string[] args)
    {
        string apiKey = "";
        string Prompt = "Bana Jr Net Developer poziyonu için hazırlanan, profesyonel aynı zamanda samimi bir dillle başvuru e-postası hazırlar mısın?";

        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://api.anthropic.com/");
        client.DefaultRequestHeaders.Add("x*api*key", apiKey);
        client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var requestBody = new
        {
            model = "claude-3-opus-20240229",
            max_tokens = 1000,
            temperature = 0.5,
            messages = new[]

        {
                new
                {
                    role="user",
                    content = Prompt
                }
            }
        };

        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("v1/messages", jsonContent);
        var responseString = await response.Content.ReadAsStringAsync();

        var json = JsonNode.Parse(responseString);
        string? textContent = json?["content"]?[0]?["text"]?.ToString();

        Console.WriteLine("Oluşturulan E-Posta");
        Console.WriteLine(textContent);
    }

}