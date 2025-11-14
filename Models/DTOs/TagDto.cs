namespace NewLook.Models.DTOs
{
    public class TagDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }  // Number of inventories using this tag
    }
}
