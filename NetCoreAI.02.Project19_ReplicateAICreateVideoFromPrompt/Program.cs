using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("🎬 Replicate AI ile Video Üretici Uygulaması");
        Console.Write("Please Input Here Prompt Text: ");
        string prompt = Console.ReadLine();

        string apiKey = "";
        string version = "9f747673945c62801b13b84701c783929c0ee784e4748ec062204894dda1a351";

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", apiKey);

        // Bölüm 1 - Üretim İsteği
        var body = new
        {
            version,
            input = new
            {
                prompt,
                num_frames = 24,
                fps = 8,
                guidance_scale = 12.5,
                num_inference_steps = 50,
                width = 576,
                height = 320
            }
        };

        var json = JsonSerializer.Serialize(body);
        var response = await client.PostAsync(
            "https://api.replicate.com/v1/predictions",
            new StringContent(json, Encoding.UTF8, "application/json")
        );

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("API Hatası: " + await response.Content.ReadAsStringAsync());
            return;
        }

        var pred = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        string id = pred.RootElement.GetProperty("id").GetString();

        Console.WriteLine("📸 Video üretiliyor...");

        // Bölüm 2 - Durum Sorgulama Döngüsü
        string status = "";
        string videoUrl = "";

        while (status != "succeeded")
        {
            await Task.Delay(5000);
            var chk = await client.GetAsync($"https://api.replicate.com/v1/predictions/{id}");
            var chkJson = JsonDocument.Parse(await chk.Content.ReadAsStringAsync());

            status = chkJson.RootElement.GetProperty("status").GetString();
            Console.WriteLine($"⌛ Durum: {status}");

            if (status == "failed")
            {
                Console.WriteLine("Üretim başarısız oldu");
                return;
            }

            if (status == "succeeded")
            {
                var output = chkJson.RootElement.GetProperty("output");
                videoUrl = output.ValueKind == JsonValueKind.Array
                    ? output[0].GetString()
                    : output.GetString();
            }
        }

        Console.WriteLine($"💎 Video hazır: {videoUrl}");

        // Bölüm 3 - İndirme
        using var stream = await client.GetStreamAsync(videoUrl);
        await using var file = File.Create("generated_video.mp4");
        await stream.CopyToAsync(file);
        Console.WriteLine("🎉 Video indirildi -> generated_video.mp4");

        // Bölüm 4 - Otomatik Aç
        Process.Start(new ProcessStartInfo
        {
            FileName = "generated_video.mp4",
            UseShellExecute = true
        });
    }
}