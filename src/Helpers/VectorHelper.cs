using AIApp.Lib.Entities;
using OllamaSharp;
using OllamaSharp.Models;

namespace AIApp.Helpers
{
    public static class VectorHelper
    {
        private static int Retries = 0;
        private static readonly int MaxRetries = 3;
        private static readonly int Delay = 1000;
        public static async Task<float[]> VectorizeAsync(string content)
        {
            OllamaApiClient ollama = new("http://host.docker.internal:11434", "mxbai-embed-large:latest");
            EmbedRequest request = new()
            {
                Input = [content]
            };
            var response = await ollama.EmbedAsync(request);
            if (response is null)
            {
                if (Retries < MaxRetries)
                {
                    Retries++;
                    await Task.Delay(Delay);
                    return await VectorizeAsync(content);
                }
                else
                {
                    throw new Exception("Failed to vectorize content after multiple attempts.");
                }
            }
            else
            {
                return response.Embeddings[0];
            }
        }
        public static async Task<List<E>> FindSimilarAsync<E>(string content, List<E> list, int count = 5, double threshold = 0.5) where E : VectorEntity
        {
            float[] vector = await VectorizeAsync(content);
            return [.. list.OrderByDescending(e => CosineSimilarity(e.Vector, vector))
                .Where(e => CosineSimilarity(e.Vector, vector) >= threshold).Take(count)];
        }
        public static List<E> FindSimilar<E>(float[] vector, List<E> list, int count = 5, double threshold = 0.5) where E : VectorEntity
        {
            return [.. list.OrderByDescending(e => CosineSimilarity(e.Vector, vector))
                .Where(e => CosineSimilarity(e.Vector, vector) >= threshold).Take(count)];
        }
        private static double CosineSimilarity(float[] vectorA, float[] vectorB)
        {
            if (vectorA.Length != vectorB.Length)
            {
                throw new ArgumentException("Vectors must be of the same length.");
            }

            float dotProduct = 0;
            float magnitudeA = 0;
            float magnitudeB = 0;

            for (int i = 0; i < vectorA.Length; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
                magnitudeA += vectorA[i] * vectorA[i];
                magnitudeB += vectorB[i] * vectorB[i];
            }

            if (magnitudeA == 0 || magnitudeB == 0)
            {
                return 0;
            }

            return dotProduct / (Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
        }
    }
}