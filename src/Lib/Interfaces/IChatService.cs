using OllamaSharp.Models.Chat;

namespace AIApp.Lib.Interfaces
{
    public interface IChatService
    {
        event Action OnUpdate;
        List<Message> Messages { get; }
        void SendMessage(string message);
    }
}