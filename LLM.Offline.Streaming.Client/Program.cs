using System.Net.Http.Json;
using System.Text.Json;

namespace LLM.Offline.Streaming.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:7102");

            while (true)
            {
                Console.Write("User:");
                string userPrompt = Console.ReadLine();

                Console.WriteLine();
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/chats")
                {
                    Content = JsonContent.Create(userPrompt)
                };

                using var response = await httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                await using var responseStream = await response.Content.ReadAsStreamAsync();

                var jsonOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                Console.Write("System");
                await foreach (var token in JsonSerializer.DeserializeAsyncEnumerable<string>(responseStream, jsonOptions))
                {
                    Console.Write(token);
                }

            }
            Console.ReadLine();
        }
    }
}
