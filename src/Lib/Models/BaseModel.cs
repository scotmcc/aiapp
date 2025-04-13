namespace AIApp.Lib.Models
{
    public class BaseModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
    public class VectorModel : BaseModel
    {
        public string Content { get; set; } = string.Empty;
        public string[] Keywords { get; set; } = [];
    }
}