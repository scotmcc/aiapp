using System.Text.Json;
using AIApp.Lib.Interfaces;
using OllamaSharp.Models.Chat;

namespace AIApp.Lib.Services
{
    public class VoiceService : IVoiceService
    {
        public event Action<string>? OnUpdate;
        private int Retries = 0;
        public Tool TextToSpeechTool => new()
        {
            Function = new()
            {
                Name = "speak_text",
                Description = "Use this tool to send a personal voice message to the user.",
                Parameters = new()
                {
                    Type = "object",
                    Properties = new()
                    {
                        { "text", new() { Type = "string", Description = "Text to convert to speech" } },
                    },
                    Required = ["text"]
                }
            }
        };
        private bool FileExists(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            while (!File.Exists(path))
            {
                if (Retries > 10)
                {
                    Console.WriteLine($"File {path} not found after 10 retries, giving up...");
                    Retries = 0;
                    return false;
                }
                Console.WriteLine($"File {path} not found, retrying...");
                Thread.Sleep(1000);
                Retries++;
            }
            Console.WriteLine($"File {path} found");
            return true;
        }
        public async Task<string> GenerateAudio(string text)
        {
            Console.WriteLine($"Generating audio for text: {text}");
            HttpClient client = new()
            {
                BaseAddress = new Uri("http://host.docker.internal:8000"),
                Timeout = TimeSpan.FromSeconds(90),

            };
            var response = await client.GetAsync($"/?text={text}");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Audio generated successfully");
                Console.WriteLine($"Response: {JsonSerializer.Serialize(await response.Content.ReadAsStringAsync())}");
                var json = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(json);
                var root = jsonDoc.RootElement;
                var message = root.GetProperty("message").GetString();
                var status = root.GetProperty("status").GetString();
                var file_name = root.GetProperty("file_name").GetString();
                Console.WriteLine($"Message: {message}");
                Console.WriteLine($"Status: {status}");
                Console.WriteLine($"Output path: {file_name}");
                if (string.IsNullOrEmpty(file_name))
                {
                    throw new Exception("File name is empty");
                }
                string output_path = "/app/wwwroot/audio/" + file_name;
                if (!string.IsNullOrEmpty(output_path) && FileExists(output_path))
                {
                    Console.WriteLine($"File {output_path} exists");
                }
                else
                {
                    Console.WriteLine($"File {output_path} does not exist");
                }
                OnUpdate?.Invoke($"/audio/{file_name}");
                return message ?? "Error generating audio";
            }
            else
            {
                throw new Exception($"Error generating audio: {response.StatusCode}");
            }
        }
    }
}