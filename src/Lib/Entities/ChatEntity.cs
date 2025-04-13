namespace AIApp.Lib.Entities
{
    public class ChatEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public ICollection<MessageEntity> Messages { get; set; } = [];
    }
    public class MessageEntity : VectorEntity
    {
        public string Role { get; set; } = string.Empty;
        public Guid ChatId { get; set; }
        public ChatEntity Chat { get; set; } = null!;
    }
}