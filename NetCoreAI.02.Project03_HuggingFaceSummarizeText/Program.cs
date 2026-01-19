

using System.Net;
using System.Text;
using System.Text.Json;

Console.Write("Enter your text here:");

var apiKey = "";
var inputText = Console.ReadLine();

var requestData = new
{
    inputs = inputText
};
var json = JsonSerializer.Serialize(requestData);
var Content = new StringContent(json, Encoding.UTF8, "application/json");

using var client = new HttpClient();
client.DefaultRequestHeaders.Authorization = new AuthorizationHeaderValue("Bearer", apiKey);

var response = await client.PostAsync("https://api-inference.huggingface.co/models/facebook/bart-large-cnn", Content);
var responseContent = await response.Content.ReadAsStringAsync();

Console.WriteLine(" Text Summarize: ");
Console.WriteLine(responseContent);











