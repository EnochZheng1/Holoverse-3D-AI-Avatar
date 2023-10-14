using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

class Program
{
    private const string API_URL = "https://api.openai.com/v1/chat/completions";
    private const string API_KEY = "key"; // Replace with your actual API key

    static async System.Threading.Tasks.Task Main(string[] args)
    {
        var chatLog = new JArray();

        while (true)
        {
            Console.Write("You: ");
            string userInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userInput))
                continue;

            // Quitting mechanism
            if (userInput.Trim().ToLower() == "exit")
            {
                Console.WriteLine("Exiting...");
                break;
            }

            chatLog.Add(new JObject
            {
                ["role"] = "user",
                ["content"] = userInput
            });

            string modelResponse = await SendMessageToOpenAI(chatLog);

            if (!string.IsNullOrWhiteSpace(modelResponse))
            {
                Console.WriteLine($"GPT-3: {modelResponse}");
                chatLog.Add(new JObject
                {
                    ["role"] = "assistant",
                    ["content"] = modelResponse
                });
            }
        }
    }

    static async Task<string> SendMessageToOpenAI(JArray chatLog)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {API_KEY}");

            var requestData = new
            {
                model = "gpt-3.5-turbo",
                messages = chatLog
            };

            var response = await client.PostAsync(
                API_URL,
                new StringContent(JObject.FromObject(requestData).ToString(), Encoding.UTF8, "application/json")
            );

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var responseObject = JObject.Parse(responseBody);
                return responseObject["choices"]?[0]?["message"]?["content"]?.ToString();
            }
            else
            {
                var errorResponseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed with status code: {response.StatusCode}\nError Details: {errorResponseBody}");
                return null;
            }
        }
    }
}
