using System.ComponentModel.DataAnnotations;

namespace AIApp.Lib.Entities
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
    public class VectorEntity : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public float[] Vector { get; set; } = [];
        public string[] Keywords { get; set; } = [];
    }
}