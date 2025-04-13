using AIApp.Lib.Models;
using OllamaSharp.Models.Chat;

namespace AIApp.Lib.Interfaces
{
    public interface IMemoryService
    {
        event Action<string> OnUpdate;
        Tool SaveMemoryTool { get; }
        Tool FindMemoryTool { get; }
        Task CreateMemory(MemoryModel memory);
        Task CreateMemories(List<MemoryModel> memories);
        Task<List<MemoryModel>> FindMemories(string content, int count = 5, double threshold = 0.5);
    }
}