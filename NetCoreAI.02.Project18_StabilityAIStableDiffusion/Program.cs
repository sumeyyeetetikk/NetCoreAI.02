using System.Net.Http.Headers;
using System.Text;

class Program
{
    static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("🤖 Prompt'tan Görsel Üretici - Stability AI SD3");
        Console.Write("Prompt girin: ");
        string prompt = Console.ReadLine();

        string apiKey = "";
        string apiUrl = "https://api.stability.ai/v2beta/stable-image/generate/sd3";

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("image/*"));

        var formData = new MultipartFormDataContent();

        // 🔹 PROMPT
        var promptContent = new StringContent(prompt, Encoding.UTF8);
        promptContent.Headers.ContentDisposition =
            new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"prompt\""
            };
        formData.Add(promptContent);

        // 🔹 WIDTH
        var widthContent = new StringContent("512");
        widthContent.Headers.ContentDisposition =
            new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"width\""
            };
        formData.Add(widthContent);

        // 🔹 HEIGHT
        var heightContent = new StringContent("512");
        heightContent.Headers.ContentDisposition =
            new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"height\""
            };
        formData.Add(heightContent);

        // 🔹 FORMAT
        var formatContent = new StringContent("jpeg");
        formatContent.Headers.ContentDisposition =
            new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"output_format\""
            };
        formData.Add(formatContent);

        var response = await httpClient.PostAsync(apiUrl, formData);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("❌ Hata: " + response.StatusCode);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            return;
        }

        var imageBytes = await response.Content.ReadAsByteArrayAsync();
        string fileName = $"generated_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";

        await File.WriteAllBytesAsync(fileName, imageBytes);
        Console.WriteLine($"✅ Görsel oluşturuldu: {fileName}");
    }
}
