using System.Text.Json;
using AIApp.Lib.Interfaces;
using AIApp.Lib.Models;
using OllamaSharp;
using OllamaSharp.Models.Chat;

namespace AIApp.Lib.Services
{
    public class ChatService(OllamaApiClient ollama, IVoiceService voice, IMemoryService memory) : IChatService
    {
        private readonly OllamaApiClient _ollama = ollama;
        private readonly IVoiceService _voice = voice;
        private readonly IMemoryService _memory = memory;
        public event Action? OnUpdate;
        public List<Message> Messages { get; private set; } = [];
        public async void SendMessage(string message)
        {
            List<MemoryModel> memories = await _memory.FindMemories(message, 3, 0.5);
            string memoryList = string.Join("\n", memories.Select(m => $"- {m.Subject}: {m.Content}"));
            var chatMessage = new Message
            {
                Role = "user",
                Content = $"Today is {DateTime.Now: dddd, d MMMM yyyy} at {DateTime.Now:hh:mm tt}.\n\n### Memories\n\n{memoryList}\n\nScot: {message}"
            };
            Messages.Add(chatMessage);
            OnUpdate?.Invoke();
            SendTools();
        }
        private async void SendTools()
        {
            ChatRequest request = new()
            {
                Messages = Messages,
                Model = "synthia:latest",
                Options = new()
                {
                    Temperature = 0.2f,
                },
                Tools = [_voice.TextToSpeechTool, _memory.SaveMemoryTool, _memory.FindMemoryTool],
            };
            Console.WriteLine($"Sending tool request: {JsonSerializer.Serialize(request)}");
            var response = await _ollama.ChatAsync(request).StreamToEndAsync();
            Console.WriteLine($"Received tool response: {JsonSerializer.Serialize(response)}");
            if (response is null) return;
            if (response.Message is null) return;
            if (response.Message.ToolCalls is not null && response.Message.ToolCalls.Any())
            {
                foreach (var toolCall in response.Message.ToolCalls)
                {
                    if (toolCall.Function == null) continue;
                    if (toolCall.Function.Name == "speak_text")
                    {
                        if (toolCall.Function.Arguments == null) continue;
                        if (toolCall.Function.Arguments.TryGetValue("text", out var text))
                        {
                            if (string.IsNullOrEmpty(text?.ToString())) continue;
                            var content = await _voice.GenerateAudio(text.ToString()!);
                            Messages.Add(new Message
                            {
                                Role = "tool",
                                Content = content
                            });
                        }
                    }
                    else if (toolCall.Function.Name == "save_memory")
                    {
                        if (toolCall.Function.Arguments == null) continue;
                        if (toolCall.Function.Arguments.TryGetValue("text", out var text) && toolCall.Function.Arguments.TryGetValue("subject", out var subject))
                        {
                            if (string.IsNullOrEmpty(text?.ToString()) || string.IsNullOrEmpty(subject?.ToString())) continue;
                            var memory = new MemoryModel
                            {
                                Content = text.ToString()!,
                                Subject = subject.ToString()!
                            };
                            await _memory.CreateMemory(memory);
                        }
                    }
                    else if (toolCall.Function.Name == "find_memory")
                    {
                        if (toolCall.Function.Arguments == null) continue;
                        if (toolCall.Function.Arguments.TryGetValue("text", out var text))
                        {
                            if (string.IsNullOrEmpty(text?.ToString())) continue;
                            var memories = await _memory.FindMemories(text.ToString()!, 3, 0.5);
                            string memoryList = string.Join("\n", memories.Select(m => $"- {m.Subject}: {m.Content}"));
                            Messages.Add(new Message
                            {
                                Role = "assistant",
                                Content = $"### Memories\n\n{memoryList}"
                            });
                        }
                    }
                }
                Send();
            }
            else
            {
                Messages.Add(new Message
                {
                    Role = "assistant",
                    Content = response.Message.Content
                });
                OnUpdate?.Invoke();
            }
        }
        private async void Send()
        {
            ChatRequest request = new()
            {
                Messages = Messages,
                Model = "mistral-small:24b-unc",
                Options = new()
                {
                    Temperature = 0.2f,
                }
            };
            Message response = new()
            {
                Role = "assistant"
            };
            Messages.Add(response);
            OnUpdate?.Invoke();
            Console.WriteLine($"Sending request: {JsonSerializer.Serialize(request)}");
            await foreach (var message in _ollama.ChatAsync(request))
            {
                if (message is null) continue;
                if (message.Message is null) continue;
                if (message.Message.Content is null) continue;
                response.Content += message.Message.Content;
                OnUpdate?.Invoke();
            }
            OnUpdate?.Invoke();
        }
    }
}