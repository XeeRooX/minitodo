namespace minitodo.Dtos
{
    public class GetTaskResult
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
        public bool IsDone { get; set; }
        public bool IsFavorite { get; set; }
    }
}
