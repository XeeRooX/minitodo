namespace minitodo.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
        public bool IsDone { get; set; }
        public bool IsFavorite { get; set; }
    }
}
