using NAudio.Wave;
using System.Net.Http.Headers;
using System.Speech.Synthesis;
using System.Text;
using System.Text.Json;

class Program
{
    static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        // 🔴 SADECE BURAYA KENDİ API KEY'İNİ KOY
        string apiKey = "";

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Console.WriteLine("❌ API Key bulunamadı");
            return;
        }

        Console.WriteLine("🎤 Sesli Chatbot Başladı");
        Console.WriteLine("👉 ENTER: Konuş");
        Console.WriteLine("👉 exit: Çıkış");

        while (true)
        {
            Console.ReadLine();

            string audioPath = "recorded.wav";

            Console.WriteLine("🎙️ Konuşmaya başlayın (5 sn)...");
            RecordAudio(audioPath);
            Console.WriteLine("⏹️ Kayıt tamamlandı");

            string text = await TranscribeAsync(audioPath, apiKey);
            if (string.IsNullOrWhiteSpace(text))
                continue;

            Console.WriteLine($"👤 Sen: {text}");

            if (text.Trim().ToLower() == "exit")
                break;

            string reply = await AskChatGPTAsync(text, apiKey);
            Console.WriteLine($"🤖 Bot: {reply}");

            using var synth = new SpeechSynthesizer();
            synth.Speak(reply);
        }
    }

    static void RecordAudio(string path)
    {
        using var waveIn = new WaveInEvent
        {
            DeviceNumber = 0,
            WaveFormat = new WaveFormat(16000, 1)
        };

        using var writer = new WaveFileWriter(path, waveIn.WaveFormat);

        waveIn.DataAvailable += (s, e) =>
        {
            writer.Write(e.Buffer, 0, e.BytesRecorded);
        };

        waveIn.StartRecording();
        Thread.Sleep(5000);
        waveIn.StopRecording();
    }

    static async Task<string> TranscribeAsync(string path, string apiKey)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        using var form = new MultipartFormDataContent();
        using var fs = File.OpenRead(path);

        form.Add(new StreamContent(fs), "file", "audio.wav");
        form.Add(new StringContent("whisper-1"), "model");

        var response = await client.PostAsync(
            "https://api.openai.com/v1/audio/transcriptions", form);

        var responseText = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("❌ Transcription API Hatası:");
            Console.WriteLine(responseText);
            return null;
        }

        using var doc = JsonDocument.Parse(responseText);
        return doc.RootElement.GetProperty("text").GetString();
    }

    static async Task<string> AskChatGPTAsync(string message, string apiKey)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        var payload = new
        {
            model = "gpt-4o-mini",
            messages = new[]
            {
                new { role = "system", content = "Kısa ve net cevap ver." },
                new { role = "user", content = message }
            }
        };

        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync(
            "https://api.openai.com/v1/chat/completions", content);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        return doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();
    }
}
