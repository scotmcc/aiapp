using System.Text.Json;
using System.Text.Json.Serialization;
using AIApp.Lib.Entities;
using OllamaSharp;
using OllamaSharp.Models;

namespace AIApp.Helpers
{
    public static class KeywordHelper
    {
        public class KeywordResponse
        {
            [JsonPropertyName("keywords")]
            public string[] Keywords { get; set; } = [];
        }
        private static int Retries = 0;
        private static readonly int MaxRetries = 3;
        private static readonly int Delay = 1000;
        public static async Task<string[]> ExtractKeywordsAsync(string text)
        {
            OllamaApiClient ollama = new("http://host.docker.internal:11434", "llama3.2:3b-unc");
            GenerateRequest request = new()
            {
                System = "Extract keywords from the following text. Ensure that the keywords are relevant and concise.\n\n### Response Format\n\n keywords: {\"keywords\": [\"keyword1\", \"keyword2\", ...]}",
                Prompt = text,
                Format = "json",
                Options = new()
                {
                    Temperature = 0.1f
                }
            };
            var response = await ollama.GenerateAsync(request).StreamToEndAsync();
            if (response is null)
            {
                if (Retries < MaxRetries)
                {
                    Retries++;
                    await Task.Delay(Delay);
                    return await ExtractKeywordsAsync(text);
                }
                else
                {
                    throw new Exception("Failed to extract keywords after multiple attempts.");
                }
            }
            else
            {
                var keywordResponse = JsonSerializer.Deserialize<KeywordResponse>(response.Response);
                if (keywordResponse != null)
                {
                    return keywordResponse.Keywords;
                }
                else
                {
                    throw new Exception("Failed to deserialize keyword response.");
                }
            }
        }
        public static async Task<List<E>> FindSimilarAsync<E>(string text, List<E> list, int count = 5) where E : MemoryEntity
        {
            string[] keywords = await ExtractKeywordsAsync(text);
            return [.. list.OrderByDescending(e => e.Keywords.Intersect(keywords).Count()).Take(count)];
        }
        public static List<E> FindSimilar<E>(string[] keywords, List<E> list, int count = 5) where E : MemoryEntity
        {
            return [.. list.OrderByDescending(e => e.Keywords.Intersect(keywords).Count()).Take(count)];
        }
    }
}