using OllamaSharp.Models.Chat;

namespace AIApp.Lib.Interfaces
{
    public interface IVoiceService
    {
        event Action<string>? OnUpdate;
        Tool TextToSpeechTool { get; }
        Task<string> GenerateAudio(string text);
    }
}