
using System.Net.Http.Headers;
using System.Text.Json;

class Prohram
{
    static async Task Main(string[] args)
    {
        string imagePath = "";
        string subscriptionKey = "";
        string endpoint = "";

        string requestParameters = "visualFeatures=Categories,Description,Tags,Color&Language=en;";
        string uri= " " + "?" + requestParameters;


        if (!File.Exists(imagePath))
        {
            Console.WriteLine("Görsel Dosyası Bulunamadı!" + imagePath);
            return;
        }

        byte[] imageBytes = await File.ReadAllBytesAsync(imagePath);

        using (HttpClient client = new HttpClient())
        using (ByteArrayContent content = new ByteArrayContent(imageBytes))
        {
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            HttpResponseMessage response = await client.GetAsync(uri, content);
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Azure Yanıtı: ");
                JsonDocument json = JsonDocument.Parse(result);
                var description = json.RootElement.GetProperty("description").GetProperty("captions")[0];
                string text = description.GetProperty("text").GetString();
                double confidence = description.GetProperty("confidence").GetDouble();

                Console.WriteLine($"Açıklama: {text} (Güven %{confidence * 100:0.00})");
            }
            else
            {

                Console.WriteLine("Bir hata oluştu!");
                Console.WriteLine($"Status {response.StatusCode}");
                Console.WriteLine("Yanıt: " + result);
            }
        }
    }
}