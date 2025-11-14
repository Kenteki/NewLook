namespace NewLook.Models.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public int AuthorId { get; set; }
        public string AuthorUsername { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateCommentDto
    {
        public int InventoryId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
