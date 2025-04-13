namespace AIApp.Lib.Models
{
    public class ChatModel : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        public List<MessageModel> Messages { get; set; } = [];
    }
    public class MessageModel : VectorModel
    {
        public string Role { get; set; } = string.Empty;
        public Guid ChatId { get; set; }
    }
}