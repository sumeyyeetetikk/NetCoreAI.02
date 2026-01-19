using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        string apiKey = "";
        string model = "gemini-2.5-flash";

        string endpoint =
            $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

        Console.WriteLine("Rolünüzü Seçin: ");
        Console.WriteLine("1-Psikolog");
        Console.WriteLine("2-Maç Yorumcusu");
        Console.WriteLine("3-Finansal Yatırım Uzmanı");
        Console.WriteLine("4-Tarihçi");
        Console.WriteLine("5-Turist Rehberi");
        Console.WriteLine("6-İngilizce Öğretmeni");
        Console.WriteLine();

        Console.Write("Seçiminiz: ");
        string roleChoice = Console.ReadLine();

        string rolePrompt = roleChoice switch
        {
            "1" => "Sen bir psikologsun. İnsanlara empatiyle yaklaşıyor, onları yargılamadan dinliyor ve duygusal destek ile psikolojik rehberlik sağlıyorsun. Cevaplarında sakin, anlayışlı ve destekleyici bir dil kullan.",
            "2" => "Sen bir maç yorumcusun. Futbol maçlarını taktiksel açıdan analiz ediyor, oyuncu performanslarını değerlendiriyor ve izleyicilere heyecan verici, akıcı ve anlaşılır yorumlar sunuyorsun.",
            "3" => "Sen bir finansal yatırım uzmansın. İnsanlara para yönetimi, yatırım araçları ve risk analizi konularında bilinçli, dengeli ve gerçekçi tavsiyelerde bulunuyorsun. Cevaplarında net ve öğretici ol.",
            "4" => "Sen bir tarihçisin. Tarihi olayları tarafsız bir bakış açısıyla ele alıyor, neden–sonuç ilişkileri kurarak geçmişi anlaşılır ve bilgilendirici şekilde anlatıyorsun.",
            "5" => "Sen bir turist rehberisin. Gezginlere tarihi, kültürel ve doğal güzellikler hakkında akıcı, ilgi çekici ve yol gösterici bilgiler sunuyorsun. Anlatımın sıcak ve keşfe teşvik edici olsun.",
            "6" => "Sen bir İngilizce öğretmenisin. Dil bilgisi, kelime hazinesi ve konuşma pratiği konularında öğrencilere sade, anlaşılır ve örneklerle desteklenen açıklamalar yapıyorsun.",
            _ => "Sen yardımcı bir yapay zekasın. Sorulara açık ve anlaşılır cevaplar ver."
        };

        Console.WriteLine();
        Console.Write("Sormak istediğiniz cümleyi giriniz: ");
        string userInput = Console.ReadLine();

        string finalPrompt =
            $"{rolePrompt}\n\nKullanıcıdan Gelen Soru: {userInput}";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = finalPrompt }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.PostAsync(endpoint, content);
        var responseText = await response.Content.ReadAsStringAsync();

        try
        {
            var doc = JsonDocument.Parse(responseText);

            string answer = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text") // ✅ DÜZELTİLDİ
                .GetString();

            Console.WriteLine("\nYANIT:\n" + answer);
        }
        catch
        {
            Console.WriteLine("Yanıt Hatası:\n" + responseText);
        }
    }
}
