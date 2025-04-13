using System.Text.Json.Serialization;

namespace AIApp.Lib.Models
{
    public class MemoryListModel
    {
        [JsonPropertyName("memories")]
        public List<MemoryModel> Memories { get; set; } = new();
    }
    public class MemoryModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Subject { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string[] Keywords { get; set; } = Array.Empty<string>();
    }
}